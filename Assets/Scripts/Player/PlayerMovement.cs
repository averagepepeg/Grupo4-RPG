using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Vector3 Speed = new Vector3(4f, 0f, 4f);
    [SerializeField]
    private float JumpSpeed = 4f;

    private Rigidbody rb;
    private Animator animator;
    private PlayerInput playerInput;
    private CapsuleCollider capsuleCollider;
    private CapsuleCollider2D test;

    private Vector2 moveDir;
    private Vector3 currDir;

    public bool groundCheck;
    public GameObject PowerBall;
    public GameObject FirePoint;
    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        test = GetComponent<CapsuleCollider2D>();
    }

    private void Start() 
    {
        DialogueManager.Instance.OnDialogueStart += OnDialogueStartDelegate;
        DialogueManager.Instance.OnDialogueFinish += OnDialogueFinishDelegate;
    }

    private void Update() 
    {
        moveDir.Normalize();
        rb.velocity = new Vector3(
            moveDir.x * Speed.x,
            rb.velocity.y,
            moveDir.y * Speed.z
        );
    }

    public void OnDialogueStartDelegate(Interaction interaction)
    {
        // Cambiar el Input Map al modo Dialogue
        playerInput.SwitchCurrentActionMap("Dialogue");
    }

    public void OnDialogueFinishDelegate()
    {
        // Cambiar el Input Map al modo Player
        playerInput.SwitchCurrentActionMap("Player");
    }

    private void OnMovement(InputValue value)
    {
        moveDir = value.Get<Vector2>();
        SetDir();
        if (Mathf.Abs(moveDir.x) > Mathf.Epsilon || 
            Mathf.Abs(moveDir.y) > Mathf.Epsilon)
        {
            animator.SetBool("IsWalking", true);
            animator.SetFloat("Horizontal", moveDir.x);
            animator.SetFloat("Vertical", moveDir.y);
        }else
        {
            animator.SetBool("IsWalking", false);
        }
        
    }
    private void SetDir() 
    {
        if (moveDir.x ==0 && moveDir.y == 1)
        {
            currDir = new Vector3(0, 0, 1);
            FirePoint.transform.position = new Vector3(0, 0.3f, 0.3f) + transform.position;
        }
        else if (moveDir.x == 1 && moveDir.y == 0) 
        {
            currDir = new Vector3(1, 0, 0);
            FirePoint.transform.position = new Vector3(0.3f, 0.3f, 0) + transform.position;
        }
        else if (moveDir.x == 0 && moveDir.y == -1)
        {
            currDir = new Vector3(0, 0, -1);
            FirePoint.transform.position = new Vector3(0, 0.3f, -0.3f) + transform.position;
        }
        else if (moveDir.x == -1 && moveDir.y == 0)
        {
            currDir = new Vector3(-1, 0, 0);
            FirePoint.transform.position = new Vector3(-0.3f, 0.3f, 0) + transform.position;
        }
    }
    private void OnAttack(InputValue value) 
    {
        GameObject Ball = Instantiate(PowerBall, FirePoint.transform.position, Quaternion.identity);
        Ball.GetComponent<EnergyMovement>().Direction = currDir;

    }
    private void OnJump(InputValue value) 
    {
        if(value.isPressed && groundCheck) 
        {
            rb.velocity += new Vector3(0,JumpSpeed,0);
            groundCheck = false;
        }
    }

    private void OnNextInteraction(InputValue value)
    {
        if (value.isPressed)
        {
            // Siguiente Dialogo
            DialogueManager.Instance.NextDialogue();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Dialogue dialogue = other.collider.transform.GetComponent<Dialogue>();
        if (dialogue != null)
        {
            // Iniciar Sistema de Dialogos
            DialogueManager.Instance.StartDialogue(dialogue);
        }
        if (other.gameObject.layer == 6)
        {
            groundCheck = true;
        }

    }

}

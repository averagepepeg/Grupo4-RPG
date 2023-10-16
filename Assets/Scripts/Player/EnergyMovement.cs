using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(SphereCollider))]
public class EnergyMovement : MonoBehaviour
{
    public Vector3 Direction;

    [SerializeField]
    private float Speed = 5f;
    [SerializeField]
    private float timeToDestroy = 1.5f;

    private Rigidbody rb;
    private float timer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void Start()
    {

    }

    private void Update()
    {
        rb.velocity = Direction.normalized * Speed;

        timer += Time.deltaTime;
        if (timer >= timeToDestroy)
        {
            EnergyDestroy();
        }
    }

    private void EnergyDestroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Hay una colision
        // TODO: Falta hacerle danho al jugador
        Debug.Log("Se destruye");
        EnergyDestroy();
    }
}

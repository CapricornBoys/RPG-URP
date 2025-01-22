using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Basic Setting")]
    public float force;
    public GameObject target;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        Fly2Target();
    }

    public void Fly2Target()
    {
        Vector3 distance = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(distance, ForceMode.Impulse);
    }
}

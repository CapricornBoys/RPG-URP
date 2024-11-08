using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator ani;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
    }

    private void Update()
    {
        SwichAnimation();
    }

    void SwichAnimation()
    {
        ani.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += Move2Target;
    }

    public void Move2Target(Vector3 target)
    {
        agent.destination = target;
    }
}

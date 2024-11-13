using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator ani;

    private GameObject attackTarget; //攻击目标
    private float lastAttackTime; // 攻击间隔

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
    }

    private void Update()
    {
        SwichAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    void SwichAnimation()
    {
        ani.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += Move2Target;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }

    

    public void Move2Target(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;        
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        transform.LookAt(attackTarget.transform);

        //TODO: 攻击距离
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > 1)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        // 攻击

        if (lastAttackTime < 0)
        {
            ani.SetTrigger("Attack");

            // cd
            lastAttackTime = 0.5f;
        }

    }
}

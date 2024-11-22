using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator ani;
    private CharacterStats characterStats;

    private GameObject attackTarget; //攻击目标
    private float lastAttackTime; // 攻击间隔

    private bool isDead;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if (isDead)
            GameManager.Instance.NotifyObserver();

        SwichAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    void SwichAnimation()
    {
        ani.SetFloat("Speed", agent.velocity.sqrMagnitude);
        ani.SetBool("Death", isDead);
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += Move2Target;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RigisterPlayer(characterStats);
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
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        transform.LookAt(attackTarget.transform);

        
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        // 攻击

        if (lastAttackTime < 0)
        {
            ani.SetBool("Critical", characterStats.isCritical);
            ani.SetTrigger("Attack");

            // cd
            lastAttackTime = characterStats.attackData.coolDown;
        }

    }


    // Animation Event

    void Hit()
    {
        var targetStats = attackTarget.GetComponent<CharacterStats>();

        targetStats.TakeDamage(characterStats, targetStats);
    }
}

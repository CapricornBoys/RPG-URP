using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    [Header("Basic Setting")]
    public float sightRadius; // 发现范围

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        SwitchStates();
    }

    void SwitchStates()
    {
        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            Debug.Log("Found Player");
        }


        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                break;
            case EnemyStates.DEAD:
                break;
        }
    }


    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}

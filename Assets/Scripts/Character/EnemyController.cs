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
    private Animator anim;
    private Collider collider;

    private CharacterStats characterStats;

    [Header("Basic Setting")]
    public float sightRadius; // 发现范围
    private GameObject attackTarget; // 目标
    public bool isGuard;
    private float speed;

    public float lookAtTime; // 停留巡视时间
    private float remainLookAtTime;

    private float lastAttackTime;

    [Header("Patrol State")]
    public float patrolRange;

    private Vector3 wayPoint; // 随机巡逻位置

    private Vector3 guardPos; // 生成位置
    private Quaternion guardRotation; // 角度


    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed;

        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }
    

    void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            //Debug.Log("Found Player");
        }


        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.1f);
                    }
                }

                break;
            case EnemyStates.PATROL:

                isChase = false;
                agent.speed = speed * 0.5f;

                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;

                    GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                ChaseTarget();
                break;
            case EnemyStates.DEAD:
                collider.enabled = false;
                agent.enabled = false;

                Destroy(gameObject, 2f);
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
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    void ChaseTarget()
    {
        isWalk = false;
        isChase = true;

        agent.speed = speed;

        // 脱战
        if (!FoundPlayer())
        {
            isFollow = false;


            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }
            else if (isGuard)
                enemyStates = EnemyStates.GUARD;
            else
                enemyStates = EnemyStates.PATROL;
        }
        else
        {
            // 追击
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;
        }


        // 是否在攻击范围
        if (TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow = false;
            agent.isStopped = true;

            if (lastAttackTime < 0)
            {
                lastAttackTime = characterStats.attackData.coolDown;
            }

            // 暴击判断
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;

            // 执行攻击
            Attack();
        }

    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) < characterStats.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) < characterStats.attackData.skillRange;
        else
            return false;
    }

    // 获取随机点
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        // 判断是否为可移动点
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }


    // Animation Event

    public void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}

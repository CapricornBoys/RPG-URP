using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25f;

    public GameObject rockPrefab;
    public Transform handPos;


    /// <summary>
    ///  Animation Event
    /// </summary>
    public void KickOff()
    {
        if (attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 distance = (attackTarget.transform.position - transform.position).normalized;
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = distance * kickForce;
            Debug.Log("velocity" + attackTarget.GetComponent<NavMeshAgent>().velocity + "\n----------------------");

            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }


    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}

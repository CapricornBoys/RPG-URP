using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack", menuName = "Attack/Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown; //cd
    public int minDamge; // 最小攻击数值
    public int maxDamge; // 最大攻击数值

    public float criticalMultiplier; // 暴击系数
    public float criticalChance; // 暴击率
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data", menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    // 血量
    public int maxHealth;
    public int currentHealth;
    // 防御
    public int baseDefence;
    public int currentDefence;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Stats")]
public class CharacterStat : ScriptableObject
{
    public float attack_damage;
    public int max_hp;
    public int armor;
    public float movespeed;
}

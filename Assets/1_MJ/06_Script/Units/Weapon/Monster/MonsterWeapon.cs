using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterWeapon : MonoBehaviour
{
    public float AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    [SerializeField] protected Monster owner;
    [SerializeField] protected float attackDamage;
    public float originalDamage { get; protected set; }

    public abstract void Attack();
    public abstract void StopAttack();
}

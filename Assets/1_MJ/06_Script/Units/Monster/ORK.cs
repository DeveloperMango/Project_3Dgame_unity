using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;

public class ORK : Monster
{
    void OnEnable()
    {
        currentHP = maxHP;
        stateMachine?.ChangeState(CharacterController.StateName.ENEMY_MOVE);
    }

    void Start()
    {
        InitSettings();
        Target = Player.Instance.transform;
    }
}

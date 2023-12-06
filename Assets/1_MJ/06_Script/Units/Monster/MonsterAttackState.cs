using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : CharacterController.BaseState
{
    private Monster monster;
    public bool isAttack { get; set; }
    public bool isCheckedPlayerPosition { get; set; }
    public Quaternion targetAngle { get; private set; }
    private float timer = 0f;
    private const float ROTATE_TIME = 0.5f;

    public MonsterAttackState(Monster monster)
    {
        this.monster = monster;
    }

    public override void OnEnterState()
    {
        isAttack = false;
        Vector3 direction = (monster.Target.position - monster.transform.position).normalized;
        targetAngle = Quaternion.LookRotation(direction);
        isCheckedPlayerPosition = false;
    }

    public override void OnExitState()
    {
        isAttack = false;
        isCheckedPlayerPosition = false;
        monster.isCooltimeDone = true;
        monster.Weapon?.StopAttack();
    }

    public override void OnFixedUpdateState()
    {
    }

    public override void OnUpdateState()
    {
        bool isOverRange = Vector3.Distance(monster.transform.position, monster.Target.position) > monster.AttackRange;
        Vector3 direction = (monster.Target.position - monster.transform.position).normalized;
        
        if (isOverRange && !isAttack)
        {
            monster.stateMachine.ChangeState(CharacterController.StateName.ENEMY_MOVE);
            return;
        }

        if (!isAttack && !isCheckedPlayerPosition)
        {
            isCheckedPlayerPosition = true;
            targetAngle = Quaternion.LookRotation(direction);
            timer = 0f;
            return;
        }

        if (Quaternion.Angle(monster.transform.rotation, targetAngle) > 5f && !isAttack && timer < ROTATE_TIME)
        {
            monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, targetAngle, monster.RotationSpeed * 2 * Time.deltaTime);
            timer += monster.RotationSpeed * 1.5f * Time.deltaTime;
            return;
        }

        if (!isAttack)
        {
            isAttack = true;
            monster.isCooltimeDone = false;
            monster.transform.rotation = targetAngle;
            monster.Weapon?.Attack();
        }
    }
}

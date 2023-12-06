using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitState : CharacterController.BaseState
{

    public bool IsHit { get; set; }
    private readonly int damagedAnimation = Animator.StringToHash("Damaged");
    protected Monster monster;
    protected float timer;

    public MonsterHitState(Monster monster)
    {
        this.monster = monster;
    }

    public override void OnEnterState()
    {
        monster.animator.SetTrigger(damagedAnimation);

        IsHit = true;
        timer = 0f;

        monster.rigidBody.isKinematic = false;
        monster.agent.isStopped = true;

        Vector3 direction = (monster.transform.position - Player.Instance.transform.position).normalized;
        var knockBackPower = Player.Instance.weaponManager.Weapon.KnockBackPower;

        monster.rigidBody.AddForce(direction* knockBackPower, ForceMode.Impulse);

        for (int i = 0; i < monster.skinnedMeshRenderers.Length; i++)
        {
            monster.skinnedMeshRenderers[i].material.color = Color.red;
        }
    }


    public override void OnExitState()
    {
        monster.rigidBody.velocity = Vector3.zero;

        monster.rigidBody.isKinematic = true;
        monster.agent.isStopped = false;
        IsHit = false;

        for (int i = 0; i < monster.skinnedMeshRenderers.Length; i++)
        { 
            monster.skinnedMeshRenderers[i].material.color = monster.originColors[i];
        }


    }

    public override void OnFixedUpdateState()
    {
    }


    public override void OnUpdateState()
    {
        timer += Time.deltaTime;

        if (timer >= 0.25f && IsHit)
        {
            IsHit = false;
            monster.rigidBody.velocity = Vector3.zero;
        }

        else if (timer >= monster.TetanyTime)
        {
            if (monster.IsBoss)
            {
                monster.stateMachine.ChangeState(CharacterController.StateName.ENEMY_CHARGE);
                return;
            }

            monster.stateMachine.ChangeState(CharacterController.StateName.ENEMY_MOVE);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveState : CharacterController.BaseState
{
    public const float CONVERT_UNIT_VALUE = 0.01f;
    public const float DEFAULT_CONVERT_MOVESPEED = 3f;
    public const float DEFAULT_ANIMATION_PLAYSPEED = 0.9f;
    public readonly int moveAnimation = Animator.StringToHash("Move");
    public readonly int moveSpeedAnimation = Animator.StringToHash("MoveSpeed");
    private Monster monster;

    public MonsterMoveState(Monster monster)
    {
        this.monster = monster;
    }

    public override void OnEnterState()
    {
        monster.agent.isStopped = false;
        monster.rigidBody.isKinematic = true;
    }

    public override void OnExitState()
    {
        monster.agent.isStopped = true;
        monster.animator.SetBool(moveAnimation, false);
    }
    public override void OnFixedUpdateState()
    {

    }
    public override void OnUpdateState()
    {
        if (monster.agent.enabled)
        {
            if (monster.IsDetected)
            {
                monster.agent.isStopped = false;
                float currentMoveSpeed = monster.MoveSpeed * CONVERT_UNIT_VALUE;
                float moveAnimationSpeed = DEFAULT_ANIMATION_PLAYSPEED + GetAnimationSyncWithMovement(currentMoveSpeed);

                monster.agent.speed = currentMoveSpeed;
                monster.animator.SetFloat(moveSpeedAnimation, moveAnimationSpeed);
                monster.animator.SetBool(moveAnimation, true);
                monster.agent.SetDestination(monster.Target.position);
                LookAtMovingDirection();
                return;
            }

            if (monster.IsAlived || monster.IsWithinAttackRange)
            {
                if (monster.isCooltimeDone) {
                    if (Player.Instance.IsDied)
                        return;
                    monster.stateMachine.ChangeState(CharacterController.StateName.ENEMY_ATTACK);
                    monster.animator.SetBool(moveAnimation, false);
                }

            }
        }
    }

    protected void LookAtMovingDirection()
    {
        Vector3 direction = monster.agent.desiredVelocity;
        direction.Set(direction.x, 0f, direction.z);

        // 방향 벡터가 제로 벡터가 아닌지 확인
        if (direction != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, targetAngle, monster.RotationSpeed * Time.deltaTime);
        }
    }



    protected float GetAnimationSyncWithMovement(float changedMoveSpeed)
    {
        if (monster.IsAlived)
            return -DEFAULT_ANIMATION_PLAYSPEED;

        // (바뀐 이동 속도 - 기본 이동속도) * 0.1f
        return (changedMoveSpeed - DEFAULT_CONVERT_MOVESPEED) * 0.1f;
    }
}

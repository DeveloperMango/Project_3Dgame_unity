using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using CharacterController;


[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    public Player player { get; private set; }
    public Vector3 inputDirection { get; private set; }
    public Vector3 calculatedDirection { get; private set; }
    public Vector3 gravity { get; private set; }
    public Vector3 MouseDirection { get; private set; }

    public DashState dashState;
    public AttackState attackState;


    void Start()
    {
        player = GetComponent<Player>();
        attackState = player.stateMachine.GetState(StateName.ATTACK) as AttackState;
        dashState = player.stateMachine.GetState(StateName.DASH) as DashState;
    }

    void Update()
    {
        calculatedDirection = GetDirection(player.MoveSpeed * MoveState.CONVERT_UNIT_VALUE);
        ControlGravity();
    }

    protected Vector3 GetDirection(float currentMoveSpeed)
    {
        return inputDirection == Vector3.zero ? Vector3.zero : inputDirection.normalized;
    }

    protected void ControlGravity()
    {
        gravity = Vector3.down * Mathf.Abs(player.rigidBody.velocity.y);
        player.rigidBody.useGravity = true;
    }

    public void OnClickLeftMouse(InputAction.CallbackContext context)
    {
        if (player.IsDied || player.stateMachine.CurrentState is HitState)
            return;

        if (context.performed && context.interaction is PressInteraction)
        {
            MouseDirection = GetMouseWorldPosition();

            bool isAvailableAttack = (!dashState.IsDash && !attackState.IsAttack) && (player.weaponManager.Weapon.ComboCount < 3);

            if (isAvailableAttack)
            {
                LookAt(MouseDirection);
                player.stateMachine.ChangeState(StateName.ATTACK);
            }
        }
    }

    protected Vector3 GetMouseWorldPosition()
    {
        if (player.IsDied || player.stateMachine.CurrentState is HitState)
            return Vector3.zero;

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 5f);  // Raycast 시각화

        if (Physics.Raycast(ray, out RaycastHit HitInfo, Mathf.Infinity))
        {
            Vector3 target = HitInfo.point;
            Vector3 myPosition = new Vector3(transform.position.x, 0f, transform.position.z);
            target.Set(target.x, 0f, target.z);
            return (target - myPosition).normalized;
        }

        return Vector3.zero;
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {

        if (player.IsDied || player.stateMachine.CurrentState is HitState)
            return;

        // 대시 키인 스페이스 바를 눌렀다 떼었을 경우에 실행되도록 해주었습니다.
        if (context.performed && context.interaction is PressInteraction)
        {
            // 대시 입력을 막아야 하는 상황이 있을 경우 return;
            if (dashState.CurrentDashCount >= player.DashCount)
                return;

            // 대시 중에 버퍼에 입력 가능한 프레임일 때 입력 받을 경우
            if (dashState.CanAddInputBuffer)
            {
                dashState.CurrentDashCount++;
                dashState.inputDirectionBuffer.Enqueue(calculatedDirection);
                return;
            }

            // Idle 상태에서 대시를 입력받을 경우
            if (!dashState.IsDash)
            {
                dashState.CurrentDashCount++;
                dashState.inputDirectionBuffer.Enqueue(calculatedDirection);
                player.stateMachine.ChangeState(StateName.DASH);
            }
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {

        if (player.IsDied || player.stateMachine.CurrentState is HitState)
        {
            inputDirection = Vector3.zero;
            return;
        }


        Vector2 input = context.ReadValue<Vector2>();
        inputDirection = new Vector3(input.x, 0f, input.y);
    }

    public void LookAt(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            transform.rotation = targetAngle;
        }
    }

}
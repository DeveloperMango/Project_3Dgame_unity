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
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 5f);  // Raycast �ð�ȭ

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

        // ��� Ű�� �����̽� �ٸ� ������ ������ ��쿡 ����ǵ��� ���־����ϴ�.
        if (context.performed && context.interaction is PressInteraction)
        {
            // ��� �Է��� ���ƾ� �ϴ� ��Ȳ�� ���� ��� return;
            if (dashState.CurrentDashCount >= player.DashCount)
                return;

            // ��� �߿� ���ۿ� �Է� ������ �������� �� �Է� ���� ���
            if (dashState.CanAddInputBuffer)
            {
                dashState.CurrentDashCount++;
                dashState.inputDirectionBuffer.Enqueue(calculatedDirection);
                return;
            }

            // Idle ���¿��� ��ø� �Է¹��� ���
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
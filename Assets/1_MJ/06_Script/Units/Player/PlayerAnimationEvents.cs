using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;

public class PlayerAnimationEvents: MonoBehaviour
{
    public float CurrentCoolTime { get; private set; } = 0f;
    public Dictionary<string, IEffect> myWeaponEffects { get; private set; }
    
    private DashState dashState;
    private AttackState attackState;

    private Coroutine dashCoolTimeCoroutine;

    #region #Unity Functions
    void Awake()
    {
        myWeaponEffects = new Dictionary<string, IEffect>();
    }

    void Start()
    {
        dashState = Player.Instance.stateMachine.GetState(StateName.DASH) as DashState;
        attackState = Player.Instance.stateMachine.GetState(StateName.ATTACK) as AttackState;
    }

    void Update()
    {
/*        if (attackState.IsAttack)
        {
            Vector3 velocity = Player.Instance.rigidBody.velocity;
            Player.Instance.rigidBody.velocity = new Vector3(velocity.x, 0f, velocity.z);
        }*/
    }
    #endregion

    public void OnDestroyEffect()
    {
        if (myWeaponEffects.TryGetValue(Player.Instance.weaponManager.Weapon.Name, out IEffect weapon))
        {
            weapon.DestroyEffect();
        }
    }


    public void OnStartAttack()
    {
        if (myWeaponEffects.TryGetValue(Player.Instance.weaponManager.Weapon.Name, out IEffect weapon))
        { 
            weapon.PlayComboAttackEffects();
        }
    }

    public void OnFinishedAttack()
    {
        attackState.IsAttack = false;
        Player.Instance.animator.SetBool("IsAttack", false);
        Player.Instance.stateMachine.ChangeState(StateName.MOVE);
    }

    public void OnCanDashAttack()
    {
        dashState.CanDashAttack = true;
    }

    public void OnChangeDashToDashAttack() { }

    public void OnFinishedDash()
    {
        //if (!dashAttackState.IsDashAttack)
        {
            dashState.CanDashAttack = false;

            if (dashState.inputDirectionBuffer.Count > 0)
            {
                Player.Instance.stateMachine.ChangeState(StateName.DASH);
                return;
            }

            dashState.CanAddInputBuffer = false;
            dashState.OnExitState();

            if (dashCoolTimeCoroutine != null)
                StopCoroutine(dashCoolTimeCoroutine);
            dashCoolTimeCoroutine = StartCoroutine(CheckDashReInputLimitTime(dashState.dashCooltime));
        }
    }

    public void OnCanAddToDashInputBuffer()
    {
        dashState.CanAddInputBuffer = true;
    }

    private IEnumerator CheckDashReInputLimitTime(float limitTime)
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > limitTime)
            {
                dashState.IsDash = false;
                dashState.CurrentDashCount = 0;
                Player.Instance.stateMachine.ChangeState(StateName.MOVE);
                break;
            }
            yield return null;
        }
    }
}

using CharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAxeWeapon : BaseWeapon, IEffect
{
    #region #이펙트
    [SerializeField] private GameObject[] defaultAttackEffs;
    #endregion

    #region #애니메이션
    public readonly int hashIsAttackAnimation = Animator.StringToHash("IsAttack");
    public readonly int hashAttackAnimation = Animator.StringToHash("AttackCombo");
    public readonly int hashAttackSpeedAnimation = Animator.StringToHash("AttackSpeed");
    #endregion

    public override void Attack(BaseState state)
    {
        ComboCount++;
        Player.Instance.rigidBody.velocity = Vector3.zero;
        Player.Instance.animator.SetFloat(hashAttackSpeedAnimation, AttackSpeed);
        Player.Instance.animator.SetBool(hashIsAttackAnimation, true);
        Player.Instance.animator.SetInteger(hashAttackAnimation, ComboCount);
        CheckAttackReInput(AttackState.CanReInputTime);

        float knockBackGauge = BaseWeapon.DEFAULT_KNOCKBACK_POWER;
        switch (ComboCount)
        {
            case 2:
                knockBackGauge = BaseWeapon.DEFAULT_KNOCKBACK_POWER * 3;
                break;
            case 3:
                knockBackGauge = BaseWeapon.DEFAULT_KNOCKBACK_POWER * 4;
                break;
        }

        Player.Instance.weaponManager.Weapon.KnockBackPower = knockBackGauge;
    }

    public override void ChargingAttack(BaseState state)
    {

    }

    public override void DashAttack(BaseState state)
    {

    }

    public override void Skill(BaseState state)
    {

    }

    public override void UltimateSkill(BaseState state)
    {

    }

    public void DestroyEffect()
    {
    }

    public void PlayComboAttackEffects()
    {
        int comboCount = Mathf.Clamp(ComboCount - 1, 0, 3);
        GameObject effect = Instantiate(defaultAttackEffs[comboCount]);
        Vector3 targetDirection = Player.Instance.Controller.MouseDirection;

        effect.transform.position = Player.Instance.effectGenerator.position;

        Vector3 secondAttackAdjustAngle = ComboCount == 2 ? new Vector3(0f, -90f, 0f) : Vector3.zero;
        effect.transform.rotation = Quaternion.LookRotation(targetDirection);
        effect.transform.eulerAngles += secondAttackAdjustAngle;
        effect.GetComponent<ParticleSystem>().Play();
    }
}
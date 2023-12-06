using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CharacterController;


[System.Serializable]
public abstract class BaseWeapon : MonoBehaviour
{
    public const float DEFAULT_KNOCKBACK_POWER = 3f;
    public float KnockBackPower { get; set; } = DEFAULT_KNOCKBACK_POWER;
    // ������ ���� �޺� ī��Ʈ
    public int ComboCount { get; set; }
    // �� ���⸦ �� ���� ���� ��ǥ ����
    public WeaponHandleData HandleData { get { return weaponhandleData; } }
    // �� ���⸦ ����� ���� �ִϸ�����
    public RuntimeAnimatorController WeaponAnimator { get { return weaponAnimator; } }
    public string Name { get { return _name; } }
    public float AttackDamage { get { return attackDamage; } }
    public float AttackSpeed { get { return attackSpeed; } }
    public float AttackRange { get { return attackRange; } }
    
    private Coroutine checkAttackReInputCor;


    #region #���� ����
    [Header("���� ����"), Tooltip("�ش� ���⸦ ����� ���� Local Transform �� �����Դϴ�.")]
    [SerializeField] protected WeaponHandleData weaponhandleData;

    [Header("���� ����")]
    [SerializeField] protected RuntimeAnimatorController weaponAnimator;
    [SerializeField] protected string _name;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackRange;
    #endregion


    public void SetWeaponData(string name, float attackDamage, float attackSpeed, float attackRange)
    {
        this._name = name;
        this.attackDamage = attackDamage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
    }

    public abstract void Attack(BaseState state);         // �⺻ ����
    public abstract void DashAttack(BaseState state);     // ��� ����
    public abstract void ChargingAttack(BaseState state); // ���� ����
    public abstract void Skill(BaseState state);          // ��ų
    public abstract void UltimateSkill(BaseState state);  // �ñر�


    public void CheckAttackReInput(float reInputTime)
    {
        if (checkAttackReInputCor != null)
            StopCoroutine(checkAttackReInputCor);
        checkAttackReInputCor = StartCoroutine(CheckAttackReInputCoroutine(reInputTime));
    }

    private IEnumerator CheckAttackReInputCoroutine(float reInputTime)
    {
        float currentTime = 0f;
        while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= reInputTime)
                break;
            yield return null;
        }

        ComboCount = 0;
        Player.Instance.animator.SetInteger("AttackCombo", 0);
    }
}
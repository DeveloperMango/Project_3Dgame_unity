using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;

public class Player : MonoBehaviour
{
    public static Player Instance { get { return instance; } }
    public WeaponManager weaponManager { get; private set; }
    public StateMachine stateMachine { get; private set; }
    public Rigidbody rigidBody { get; private set; }
    public Animator animator { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }
    public PlayerController Controller { get; private set; }
    public bool IsDied { get; private set; } = false;
    public PlayerAnimationEvents _PlayerAnimationEvents { get; private set; }
    public SkinnedMeshRenderer skinnedMeshRenderer { get; private set; }
    public Color originalMaterialColor { get; private set; }

    public Transform effectGenerator;

    [SerializeField]
    private Transform rightHand;
    private static Player instance;

    #region #캐릭터 스탯
    public float MaxHP { get { return maxHP; } }
    public float CurrentHP { get { return currentHP; }}
    public float Armor { get { return armor; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public int DashCount { get { return dashCount; } }

    [Header("캐릭터 스탯")]
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
    [SerializeField] protected float armor;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int dashCount;
    #endregion

    float ACTIVE_TIME = 3.0f;

    #region #Unity 함수
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            weaponManager = new WeaponManager(rightHand);
            weaponManager.unRegisterWeapon = (weapon) => { Destroy(weapon); };

            rigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            Controller = GetComponent<PlayerController>();

            _PlayerAnimationEvents = GetComponent<PlayerAnimationEvents>();


            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            originalMaterialColor = skinnedMeshRenderer.material.color;

            InitStateMachine();
            DontDestroyOnLoad(gameObject);
            return;
        }
        DestroyImmediate(gameObject);
    }


    void Update()
    {
        stateMachine?.UpdateState();
    }

    void FixedUpdate()
    {
        stateMachine?.FixedUpdateState();
    }
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            MonsterHitState hitState;

            if (Player.Instance.stateMachine.CurrentState == Player.Instance.stateMachine.GetState(StateName.ATTACK))
            {
                hitState = monster.stateMachine.GetState(StateName.ENEMY_HIT) as MonsterHitState;
                if (monster.stateMachine.CurrentState == monster.stateMachine.GetState(StateName.ENEMY_DIE) || hitState.IsHit)
                    return;

                monster?.Damaged(10);
            }
            else
            {
                return;
            }
        }
    }

    public void OnUpdateStat(float maxHP, float currentHP, float armor, float moveSpeed, int dashCount)
    {
        this.maxHP = maxHP;
        this.currentHP = currentHP;
        this.armor = armor;
        this.moveSpeed = moveSpeed;
        this.dashCount = dashCount;
    }

    private void InitStateMachine()
    {
        stateMachine = new StateMachine(StateName.MOVE, new MoveState()); // 등록
        stateMachine.AddState(StateName.DASH, new DashState(dashPower: 1.5f, dashTetanyTime: 0.5f, dashCoolTime: 0.5f));
        stateMachine.AddState(StateName.ATTACK, new AttackState());
        stateMachine.AddState(StateName.HIT, new HitState());
    }

    public void Damaged(float damage)
    {
        if (stateMachine.CurrentState is DashState || IsDied || stateMachine.CurrentState is HitState)
            return;

        float resultDamage = damage - (armor * 0.01f);
        currentHP = Mathf.Clamp((currentHP - resultDamage), 0, maxHP);

        if (Mathf.Approximately(currentHP, 0))
        {
            animator.SetTrigger("Die");
            rigidBody.velocity = Vector3.zero;
            rigidBody.isKinematic = true;
            IsDied = true;
            StartCoroutine("CorCooldown", ACTIVE_TIME);
            return;
        }
        stateMachine.ChangeState(StateName.HIT);
    }


    IEnumerator CorCooldown(float second)
    {
        float cool = second;
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // FadeInOutController.Instance.FadeOutAndLoadScene("GameOver", StageType.Unknown);
    }

}
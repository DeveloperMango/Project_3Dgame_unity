using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CharacterController;
using Unity.XR.Oculus.Input;

public class Monster : MonoBehaviour
{

    public string Name { get { return _name; } }
    public float MaxHP { get { return maxHP; } }
    public float CurrentHP { get { return currentHP; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public float Armor { get { return armor; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    public MonsterWeapon Weapon { get { return weapon; } }
    public float AttackDelay { get { return attackDelay; } }
    public float AttackRange { get { return attackRange; } }
    public float TetanyTime { get { return tetanyTime; } }


    #region #몬스터 스탯
    [Header("몬스터 스탯")]
    [SerializeField] protected string _name;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float armor;
    [SerializeField] protected MonsterWeapon weapon;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float tetanyTime;

    [SerializeField, Tooltip("적을 탐지하는 거리입니다."), Range(0.0f, float.PositiveInfinity)]
    protected float detectRange;
    [SerializeField, Tooltip("공격 사거리입니다.")]
    protected float attackRange;

    [Header("옵션")]
    [SerializeField] protected float rotationSpeed;   // 15f가 적당
    #endregion

    public Transform Target { get; protected set; }
    public NavMeshAgent agent { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody rigidBody { get; private set; }
    public StateMachine stateMachine { get; private set; }
    public SkinnedMeshRenderer[] skinnedMeshRenderers { get; private set; }
    public List<Color> originColors { get; private set; } = new List<Color>();

    protected Coroutine attackDelayCoroutine;

    public bool IsAlived { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsDetected { get; private set; }
    public bool IsWithinAttackRange { get; private set; }
    public bool isCooltimeDone { get; set; } = true;
    public bool IsBoss { get; protected set; } = false;

    #region# Unity Functions
    void Update()
    {
        agent.stoppingDistance = attackRange;
        CalculateAliveOrMoving();
        stateMachine?.UpdateState();
    }

    void FixedUpdate()
    {
        stateMachine?.FixedUpdateState();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            
            if (stateMachine.CurrentState == stateMachine.GetState(StateName.ENEMY_ATTACK))
            {
                Debug.Log(stateMachine.CurrentState);
                HitState hitState = Player.Instance.stateMachine.GetState(CharacterController.StateName.HIT) as HitState;

                if (Player.Instance.stateMachine.CurrentState == hitState)
                    return;

                Player.Instance.Damaged(Weapon.AttackDamage);
            }
        }
    }
    #endregion

    protected void InitSettings()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            originColors.Add(skinnedMeshRenderers[i].material.color);
        }

        agent.updateRotation = false;
        agent.isStopped = true;

        stateMachine = new StateMachine(StateName.ENEMY_MOVE, new MonsterMoveState(this));
        stateMachine.AddState(StateName.ENEMY_ATTACK, new MonsterAttackState(this));
        stateMachine.AddState(StateName.ENEMY_HIT, new MonsterHitState(this));
        stateMachine.AddState(StateName.ENEMY_DIE, new MonsterDieState(this));
        agent.stoppingDistance = attackRange;
    }

    public void SetStats(float maxHP, float currentHP, float moveSpeed, float armor)
    {
        this.maxHP = maxHP;
        this.currentHP = currentHP;
        this.moveSpeed = moveSpeed;
        this.armor = armor;
    }

    protected void CalculateAliveOrMoving()
    {
        if (agent.enabled)
        {
            IsAlived = agent.velocity.sqrMagnitude >= 0.1f * 0.1f && agent.remainingDistance <= agent.stoppingDistance + 0.1f;
            IsMoving = agent.desiredVelocity.sqrMagnitude >= 0.1f * 0.1f;

            var distance = Vector3.Distance(Target.position, this.transform.position);

            IsWithinAttackRange = (distance <= attackRange);
            IsDetected = (distance <= detectRange && !IsWithinAttackRange);
        }
    }

    public void LookAt(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            transform.rotation = targetAngle;
        }
    }

    public void Damaged(float damage)
    {
        if (stateMachine.CurrentState is MonsterAttackState)
            weapon?.StopAttack();

        currentHP = Mathf.Clamp(currentHP - (damage - armor * 0.01f), 0, maxHP);

        if (Mathf.Approximately(currentHP, 0f))
        {
            stateMachine.ChangeState(StateName.ENEMY_DIE);
            return;
        }

        stateMachine.ChangeState(StateName.ENEMY_HIT);
    }

    public void CheckAttackDelay()
    {
        weapon.StopAttack();

        if (attackDelayCoroutine != null)
            StopCoroutine(attackDelayCoroutine);
        attackDelayCoroutine = StartCoroutine(AttackDelayCoroutine());
    }

    private IEnumerator AttackDelayCoroutine()
    {
        float timer = 0f;

        stateMachine.ChangeState(StateName.ENEMY_MOVE);
        isCooltimeDone = false;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= attackDelay)
            {
                MonsterAttackState attackState = stateMachine.GetState(StateName.ENEMY_ATTACK) as MonsterAttackState;
                attackState.isAttack = false;
                attackState.isCheckedPlayerPosition = false;

                isCooltimeDone = true;
                break;
            }
            yield return null;
        }
    }
}


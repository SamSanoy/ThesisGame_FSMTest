using UnityEngine;

[RequireComponent(typeof(EnemyChaseAI), typeof(EnemyMeleeAttack), typeof(EnemySpecialAttacks))]
[RequireComponent(typeof(EnemyAttackRecovery))]
public class EnemyStateMachine : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        NormalAttack,
        AOEAttack,
        LongRangeAttack
    }

    [Header("Target (optional: uses EnemyChaseAI target)")]
    [SerializeField] private Transform player;

    [Header("Attack Testing (uncheck to disable an attack)")]
    [SerializeField] private bool enableNormalAttack = true;
    [SerializeField] private bool enableAOEAttack = true;
    [SerializeField] private bool enableLongRangeAttack = true;

    [Header("Transition Smoothing")]
    [Tooltip("Extra time after a Normal Attack's recovery before another Normal Attack can begin. Set to 0 for original timing.")]
    [SerializeField, Min(0f)] private float normalAttackReentryDelay = 0.12f;

    [Header("FSM Debug")]
    [SerializeField] private EnemyState currentState = EnemyState.Chase;
    [SerializeField] private bool logStateChanges = false;

    public EnemyState CurrentState => currentState;
    public bool NormalAttackEnabled => enableNormalAttack;
    public bool AOEAttackEnabled => enableAOEAttack;
    public bool LongRangeAttackEnabled => enableLongRangeAttack;

    private float nextNormalAttackTime;

    private EnemyChaseAI chaseAI;
    private EnemyMeleeAttack meleeAttack;
    private EnemySpecialAttacks specialAttacks;
    private EnemyAttackRecovery recovery;

    private void Awake()
    {
        chaseAI = GetComponent<EnemyChaseAI>();
        meleeAttack = GetComponent<EnemyMeleeAttack>();
        specialAttacks = GetComponent<EnemySpecialAttacks>();
        recovery = GetComponent<EnemyAttackRecovery>();

        if (player == null && chaseAI != null)
            player = chaseAI.player;

        if (player == null && meleeAttack != null)
            player = meleeAttack.player;

        currentState = EnemyState.Chase;
    }

    private void Start()
    {
        if (player == null)
            Debug.LogError("EnemyStateMachine: Assign the Player target in EnemyChaseAI or in the FSM Inspector.", this);
    }

    private void Update()
    {
        if (currentState != EnemyState.Chase || recovery.IsBusy || player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (enableAOEAttack &&
            specialAttacks.CanUseAOE(distance) &&
            specialAttacks.TryActivateAOE(ReturnToChase))
        {
            ChangeState(EnemyState.AOEAttack);
            return;
        }

        if (enableLongRangeAttack &&
            specialAttacks.CanUseLongRange(distance) &&
            specialAttacks.TryActivateLongRange(ReturnToChase))
        {
            ChangeState(EnemyState.LongRangeAttack);
            return;
        }

        if (enableNormalAttack &&
            Time.time >= nextNormalAttackTime &&
            meleeAttack.CanUseNormalAttack(distance) &&
            meleeAttack.TryExecuteNormalAttack(ReturnFromNormalAttack))
        {
            ChangeState(EnemyState.NormalAttack);
        }
    }

    private void ReturnFromNormalAttack()
    {
        nextNormalAttackTime = Time.time + Mathf.Max(0f, normalAttackReentryDelay);
        ReturnToChase();
    }

    private void ReturnToChase()
    {
        ChangeState(EnemyState.Chase);
    }

    private void ChangeState(EnemyState nextState)
    {
        if (currentState == nextState)
            return;

        if (logStateChanges)
            Debug.Log("Enemy FSM: " + currentState + " -> " + nextState, this);

        currentState = nextState;
    }
}
using UnityEngine;

[RequireComponent(typeof(EnemyAttackRecovery))]
public class EnemySpecialAttacks : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Attack Prefabs")]
    [SerializeField] private GameObject aoeAttackPrefab;
    [SerializeField] private GameObject longRangeAttackPrefab;

    [Header("AOE Settings")]
    [SerializeField] private float aoeRange = 1.5f;
    [SerializeField] private float aoeRecovery = 1.5f;

    [Header("Long-Range Settings")]
    [SerializeField] private float closeAttackRange = 2f;
    [SerializeField] private float projectileSpawnOffset = 1f;
    [SerializeField] private float longRangeExecution = 0.2f;
    [SerializeField] private float longRangeRecovery = 0f;

    private EnemyAttackRecovery recovery;
    private PlayerHitCounter hitCounter;
    private DisengagementCounter disengagementCounter;

    public bool HasExecutableSpecial
    {
        get
        {
            if (player == null)
                return false;

            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            return
                (aoeAttackPrefab != null &&
                 hitCounter != null &&
                 hitCounter.IsReady &&
                 distance <= aoeRange)
                ||
                (longRangeAttackPrefab != null &&
                 disengagementCounter != null &&
                 disengagementCounter.IsReady &&
                 distance > closeAttackRange);
        }
    }

    void Awake()
    {
        recovery = GetComponent<EnemyAttackRecovery>();
        hitCounter = GetComponent<PlayerHitCounter>();
        disengagementCounter =
            GetComponent<DisengagementCounter>();
    }

    void Update()
    {
        if (recovery.IsBusy || player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (hitCounter != null &&
            hitCounter.IsReady &&
            distance <= aoeRange)
        {
            ActivateAOE();
            return;
        }

        if (disengagementCounter != null &&
            disengagementCounter.IsReady &&
            distance > closeAttackRange)
        {
            ActivateLongRange();
        }
    }

    
    public void ActivateAOE()
    {
        if (aoeAttackPrefab == null ||
            hitCounter == null ||
            !hitCounter.IsReady)
            return;

        float totalAttackDuration = 1.1f;

        if (!recovery.TryBeginAttack(
            totalAttackDuration,
            aoeRecovery,
            hitCounter.OnAOERecoveryComplete))
            return;

        Instantiate(
            aoeAttackPrefab,
            transform.position,
            Quaternion.identity
        );

        hitCounter.OnAOEExecuted();

        //Debug.Log("Enemy started AOE Attack!");
    }

    public void ActivateLongRange()
    {
        if (longRangeAttackPrefab == null ||
            player == null ||
            disengagementCounter == null ||
            !disengagementCounter.IsReady)
            return;

        if (!recovery.TryBeginAttack(
            longRangeExecution,
            longRangeRecovery))
            return;

        Vector2 direction =
            ((Vector2)player.position -
            (Vector2)transform.position).normalized;

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * projectileSpawnOffset;

        GameObject projectile = Instantiate(
            longRangeAttackPrefab,
            spawnPosition,
            Quaternion.identity
        );

        EnemyLongRangeAttack attack =
            projectile.GetComponent<EnemyLongRangeAttack>();

        if (attack != null)
            attack.Initialize(direction);

        disengagementCounter.OnLongRangeExecuted();

        //Debug.Log("Enemy activated Long-Range Slow Attack!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;

        Gizmos.DrawWireSphere(
            transform.position,
            aoeRange
        );
    }
}
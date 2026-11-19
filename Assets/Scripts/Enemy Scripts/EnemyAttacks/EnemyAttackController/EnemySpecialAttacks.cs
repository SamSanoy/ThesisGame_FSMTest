using System;
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
    [SerializeField] private float projectileSpawnOffset = 1f;
    [SerializeField] private float longRangeExecution = 0.2f;
    [SerializeField] private float longRangeRecovery = 0f;

    private EnemyAttackRecovery recovery;
    private PlayerHitCounter hitCounter;
    private DisengagementCounter disengagementCounter;

    private float CloseAttackRange =>
        disengagementCounter != null
            ? disengagementCounter.EngagementRange
            : 1.25f;

    private void Awake()
    {
        recovery = GetComponent<EnemyAttackRecovery>();
        hitCounter = GetComponent<PlayerHitCounter>();
        disengagementCounter = GetComponent<DisengagementCounter>();
    }

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

            return CanUseAOE(distance) ||
                   CanUseLongRange(distance);
        }
    }

    public bool CanUseAOE(float distance)
    {
        return player != null &&
               aoeAttackPrefab != null &&
               hitCounter != null &&
               hitCounter.IsReady &&
               distance <= aoeRange;
    }

    public bool TryActivateAOE(Action onRecoveryComplete)
    {
        if (player == null)
            return false;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (!CanUseAOE(distance))
            return false;

        float totalAttackDuration = 1.1f;

        if (!recovery.TryBeginAttack(
            totalAttackDuration,
            aoeRecovery,
            () =>
            {
                hitCounter.OnAOERecoveryComplete();
                onRecoveryComplete?.Invoke();
            }))
        {
            return false;
        }

        Instantiate(
            aoeAttackPrefab,
            transform.position,
            Quaternion.identity
        );

        hitCounter.OnAOEExecuted();

        return true;
    }

    public bool CanUseLongRange(float distance)
    {
        if (player == null ||
            longRangeAttackPrefab == null ||
            disengagementCounter == null)
        {
            return false;
        }

        if (!disengagementCounter.IsReady)
            return false;

        if (distance <= CloseAttackRange)
            return false;

        return true;
    }

    public bool TryActivateLongRange(Action onRecoveryComplete)
    {
        if (player == null)
            return false;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (!CanUseLongRange(distance))
            return false;

        if (!recovery.TryBeginAttack(
            longRangeExecution,
            longRangeRecovery,
            onRecoveryComplete))
        {
            return false;
        }

        Vector2 direction =
            (Vector2)player.position -
            (Vector2)transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.down;

        direction.Normalize();

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
        {
            attack.Initialize(direction);
        }
        else
        {
            Debug.LogWarning(
                "Long-Range prefab is missing EnemyLongRangeAttack.",
                this
            );
        }

        disengagementCounter.OnLongRangeExecuted();

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(
            transform.position,
            aoeRange
        );
    }
}
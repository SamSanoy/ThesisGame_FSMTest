using System;
using UnityEngine;

[RequireComponent(typeof(EnemyAttackRecovery))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Normal Attack Settings")]
    public float attackRange = 1f;

    [Header("Hitbox")]
    public GameObject attackPrefab;
    public float attackOffset = 0.8f;
    public float attackDuration = 0.2f;

    [Header("Recovery")]
    [SerializeField] private float recoveryDuration = 0.8f;

    private EnemyAttackRecovery recovery;

    private void Awake()
    {
        recovery = GetComponent<EnemyAttackRecovery>();
    }

    public bool CanUseNormalAttack(float distance)
    {
        return player != null &&
               attackPrefab != null &&
               distance <= attackRange;
    }

    public bool TryExecuteNormalAttack(Action onRecoveryComplete)
    {
        if (player == null || attackPrefab == null)
            return false;

        float distance = Vector2.Distance(transform.position, player.position);
        if (!CanUseNormalAttack(distance))
            return false;

        if (!recovery.TryBeginAttack(attackDuration, recoveryDuration, onRecoveryComplete))
            return false;

        Vector2 direction = (Vector2)player.position - (Vector2)transform.position;
        SpawnAttack(direction);
        return true;
    }

    private void SpawnAttack(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.down;

        direction.Normalize();
        Vector2 spawnPosition = (Vector2)transform.position + direction * attackOffset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject hitbox = Instantiate(attackPrefab, spawnPosition, rotation);
        Destroy(hitbox, attackDuration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
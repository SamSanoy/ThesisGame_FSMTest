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
    private EnemySpecialAttacks specialAttacks;

    void Awake()
    {
        recovery = GetComponent<EnemyAttackRecovery>();
        specialAttacks = GetComponent<EnemySpecialAttacks>();
    }

    void Update()
    {
        if (player == null || attackPrefab == null)
            return;

        if (recovery.IsBusy)
            return;

        if (specialAttacks != null &&
            specialAttacks.HasExecutableSpecial)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
            return;

        if (!recovery.TryBeginAttack(
            attackDuration,
            recoveryDuration))
            return;

        Vector2 direction =
            player.position - transform.position;

        Attack(direction);
    }

    private void Attack(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.down;

        direction.Normalize();

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * attackOffset;

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        Quaternion rotation =
            Quaternion.Euler(0f, 0f, angle);

        GameObject hitbox = Instantiate(
            attackPrefab,
            spawnPosition,
            rotation
        );

        Destroy(hitbox, attackDuration);

        //Debug.Log("Enemy Normal Attack!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}
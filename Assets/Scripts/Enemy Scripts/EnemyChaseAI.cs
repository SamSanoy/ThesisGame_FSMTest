using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaseAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float enemyMovementSpeed = 5f;
    public float speedMultiplier = 0.8f;

    [Header("Stopping Distance")]
    public float stopDistance = 1.5f;

    private Rigidbody2D rb;
    private EnemyAttackRecovery recovery;

    private float ChaseSpeed => enemyMovementSpeed * speedMultiplier;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        recovery = GetComponent<EnemyAttackRecovery>();
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        if (recovery != null && recovery.IsBusy)
            return;

        Vector2 currentPosition = rb.position;
        Vector2 playerPosition = player.position;

        Vector2 direction = playerPosition - currentPosition;
        float distance = direction.magnitude;

        if (distance <= stopDistance)
            return;

        Vector2 moveDirection = direction.normalized;
        float moveDistance = Mathf.Min(
            ChaseSpeed * Time.fixedDeltaTime,
            distance - stopDistance
        );

        rb.MovePosition(
            currentPosition + moveDirection * moveDistance
        );
    }
}

using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLongRangeAttack : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float lifetime = 3f;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;

    [Header("Slow Effect")]
    [SerializeField] private float slowMultiplier = 0.3f;
    [SerializeField] private float slowDuration = 3f;

    private Vector2 direction;
    private Rigidbody2D rb;
    private bool hasHitPlayer;

    public void Initialize(Vector2 targetDirection)
    {
        direction = targetDirection.normalized;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        CircleCollider2D hitbox =
            GetComponent<CircleCollider2D>();

        hitbox.isTrigger = true;

        rb.gravityScale = 0f;

        hasHitPlayer = false;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            direction *
            projectileSpeed *
            Time.fixedDeltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        if (!playerHealth.HasHealth)
            return;

        hasHitPlayer = true;

        playerHealth.TakeDamage(damage);

        CharacterController2D player =
            playerHealth.GetComponent<CharacterController2D>();

        if (player == null)
        {
            player =
                playerHealth.GetComponentInParent<CharacterController2D>();
        }

        if (player != null && playerHealth.HasHealth)
        {
            player.ApplySlow(
                slowMultiplier,
                slowDuration
            );
        }

        /*Debug.Log(
            "Long-Range Attack hit player! Damage: " +
            damage +
            " | Remaining HP: " +
            playerHealth.CurrentHealth
        );*/

        Destroy(gameObject);
    }
}
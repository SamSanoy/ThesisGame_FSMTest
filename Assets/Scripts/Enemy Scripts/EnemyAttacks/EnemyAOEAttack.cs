using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAOEAttack : MonoBehaviour
{
    [Header("AOE Timing")]
    [SerializeField] private float warningDuration = 0.8f;
    [SerializeField] private float attackDuration = 0.3f;

    [Header("Warning Effect")]
    [SerializeField] private float flashSpeed = 10f;
    [SerializeField] private float minimumAlpha = 0.15f;
    [SerializeField] private float maximumAlpha = 0.6f;

    [Header("Attack Appearance")]
    [SerializeField] private float attackAlpha = 0.85f;

    private CircleCollider2D aoeCollider;
    private SpriteRenderer spriteRenderer;

    private bool attackActive = false;
    private bool hasHitPlayer = false;

    void Awake()
    {
        aoeCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        aoeCollider.isTrigger = true;

        aoeCollider.enabled = false;
    }

    void Start()
    {
        StartCoroutine(AOESequence());
    }

    private IEnumerator AOESequence()
    {
        float timer = 0f;

        while (timer < warningDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(
                minimumAlpha,
                maximumAlpha,
                (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f
            );

            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            yield return null;
        }

        //Debug.Log("AOE Attack Activated!");

        Color attackColor = spriteRenderer.color;
        attackColor.a = attackAlpha;
        spriteRenderer.color = attackColor;

        attackActive = true;
        aoeCollider.enabled = true;

        yield return new WaitForSeconds(attackDuration);

        attackActive = false;
        aoeCollider.enabled = false;

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryHitPlayer(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryHitPlayer(other);
    }

    
    private void TryHitPlayer(Collider2D other)
    {
        if (!attackActive || hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        hasHitPlayer = true;

        playerHealth.TakeDamage(10f);

        //Debug.Log("Player hit by AOE Attack!");
    }
}
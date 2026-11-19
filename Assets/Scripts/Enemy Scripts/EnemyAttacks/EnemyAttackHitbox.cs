using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAttackHitbox : MonoBehaviour
{
    [Header("Attack Damage")]

    private BoxCollider2D hitbox;
    private bool hasHitPlayer;

    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        hitbox.isTrigger = true;
    }

    private void OnEnable()
    {
        hasHitPlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
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

        playerHealth.TakeDamage(5f);

        hasHitPlayer = true;

        //Debug.Log("Enemy hit player for " + "5" + " damage.");
    }
}
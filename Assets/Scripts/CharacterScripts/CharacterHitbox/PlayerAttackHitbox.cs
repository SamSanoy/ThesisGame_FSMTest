using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerAttackHitbox : MonoBehaviour
{
    public float damage = 10f;

    private PlayerHealing playerHealing;

    private HashSet<PlayerHitCounter> hitEnemies =
        new HashSet<PlayerHitCounter>();

    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;

        playerHealing =
            GetComponentInParent<PlayerHealing>();

        if (playerHealing == null)
        {
            playerHealing =
                FindFirstObjectByType<PlayerHealing>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryHitEnemy(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryHitEnemy(other);
    }

    private void TryHitEnemy(Collider2D other)
    {
        PlayerHitCounter hitCounter =
            other.GetComponentInParent<PlayerHitCounter>();

        if (hitCounter == null)
            return;

        if (!hitEnemies.Add(hitCounter))
            return;

        hitCounter.RegisterPlayerHit();

        if (playerHealing != null)
        {
            playerHealing.RegisterSuccessfulHit();
        }

        /*Debug.Log(
            "Player successfully hit enemy!"
        );*/
    }
}
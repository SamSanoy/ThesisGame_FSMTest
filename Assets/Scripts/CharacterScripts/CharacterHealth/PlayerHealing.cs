using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerHealing : MonoBehaviour
{
    [Header("Successful Hit Healing")]
    [SerializeField] private int requiredHits = 2;
    [SerializeField] private float healPercentage = 0.05f;

    [Header("Comeback Regeneration")]
    [SerializeField] private float regenerationRate = 0.03f;
    [SerializeField] private float regenerationLimit = 0.60f;

    private PlayerHealth playerHealth;

    private int successfulHits;

    private bool isFortyPercentRevivalTier;

    private bool isRegenerating;

    public int SuccessfulHits => successfulHits;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        playerHealth.OnDamageTaken += HandleDamageTaken;
        playerHealth.OnFullHealthReached += HandleFullHealth;
        playerHealth.OnHealthDepleted += HandleHealthDepleted;
    }

    private void OnDisable()
    {
        playerHealth.OnDamageTaken -= HandleDamageTaken;
        playerHealth.OnFullHealthReached -= HandleFullHealth;
        playerHealth.OnHealthDepleted -= HandleHealthDepleted;
    }

    private void Update()
    {
        HandleComebackRegeneration();
    }

    public void RegisterSuccessfulHit()
    {
        if (!playerHealth.HasHealth)
            return;

        successfulHits++;

        /*Debug.Log(
            "Healing progress: " +
            successfulHits + "/" + requiredHits
        );*/

        if (successfulHits >= requiredHits)
        {
            successfulHits = 0;

            float healAmount =
                playerHealth.MaxHealth * healPercentage;

            playerHealth.Heal(healAmount);
        }
    }

    private void HandleDamageTaken(float damage)
    {
        successfulHits = 0;
        isRegenerating = false;

        //Debug.Log("Healing progress reset.");
    }

    private void HandleFullHealth()
    {
        isFortyPercentRevivalTier = false;
        isRegenerating = false;
    }

    private void HandleHealthDepleted()
    {
        successfulHits = 0;
        isRegenerating = false;
    }

    public void SetRevivalTier(int deathNumber)
    {
        isFortyPercentRevivalTier = deathNumber >= 3;

        isRegenerating =
            isFortyPercentRevivalTier &&
            playerHealth.HasHealth &&
            playerHealth.CurrentHealth <
            playerHealth.MaxHealth * regenerationLimit;
    }

    private void HandleComebackRegeneration()
    {
        if (!isRegenerating ||
            !isFortyPercentRevivalTier ||
            !playerHealth.HasHealth)
            return;

        float targetHealth =
            playerHealth.MaxHealth * regenerationLimit;

        if (playerHealth.CurrentHealth >= targetHealth)
        {
            isRegenerating = false;
            return;
        }

        float healAmount =
            playerHealth.MaxHealth *
            regenerationRate *
            Time.deltaTime;

        healAmount = Mathf.Min(
            healAmount,
            targetHealth - playerHealth.CurrentHealth
        );

        playerHealth.Heal(healAmount);
    }
}
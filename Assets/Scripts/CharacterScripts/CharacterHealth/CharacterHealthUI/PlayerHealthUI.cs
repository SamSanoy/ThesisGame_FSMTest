using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            UpdateHealthUI(
                playerHealth.CurrentHealth,
                playerHealth.MaxHealth
            );
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text =
                Mathf.CeilToInt(currentHealth) +
                " / " +
                Mathf.CeilToInt(maxHealth);
        }
    }
}
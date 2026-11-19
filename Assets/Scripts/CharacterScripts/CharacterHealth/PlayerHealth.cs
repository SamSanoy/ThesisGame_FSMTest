using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool HasHealth => CurrentHealth > 0f;
    public bool IsFullHealth => CurrentHealth >= MaxHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnDamageTaken;
    public event Action OnHealthDepleted;
    public event Action OnFullHealthReached;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || !HasHealth)
            return;

        float previousHealth = CurrentHealth;

        CurrentHealth = Mathf.Max(
            0f,
            CurrentHealth - damage
        );

        float actualDamage = previousHealth - CurrentHealth;

        OnDamageTaken?.Invoke(actualDamage);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        //Debug.Log("Player HP: " + CurrentHealth);

        if (CurrentHealth <= 0f)
        {
            OnHealthDepleted?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || !HasHealth)
            return;

        bool wasFullHealth = IsFullHealth;

        CurrentHealth = Mathf.Min(
            MaxHealth,
            CurrentHealth + amount
        );

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        //Debug.Log("Player healed. HP: " + CurrentHealth);

        if (!wasFullHealth && IsFullHealth)
        {
            OnFullHealthReached?.Invoke();
        }
    }

    public void RestoreHealth(float healthAmount)
    {
        CurrentHealth = Mathf.Clamp(
            healthAmount,
            0f,
            MaxHealth
        );

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
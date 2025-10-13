using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour,IDamageable
{
    [Header("Health Settings")]
    public float totalHealth = 100f;
    private float currentHealth;
    [SerializeField] bool showDamagePopup = false;
    [SerializeField] Transform damagePopUpPoint;

    // This event notifies listeners whenever health changes
    public Action<float> OnEventHealthStatusChanged;

    private void Awake()
    {
        currentHealth = totalHealth;
        FireHealthEvent();
    }

    /// <summary>
    /// Reduces health by given amount and fires event.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, totalHealth);

        FireHealthEvent();
        if(showDamagePopup)
        {
            DamagePopup.Create(damagePopUpPoint.position, (int)amount, false);
        }

        if (currentHealth <= 0f)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Adds health by given amount and fires event.
    /// </summary>
    public void AddHealth(float amount)
    {
        if (amount <= 0f) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, totalHealth);

        FireHealthEvent();
    }

    /// <summary>
    /// Fires the event to notify listeners of current health.
    /// </summary>
    private void FireHealthEvent()
    {
        OnEventHealthStatusChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Called automatically when health reaches zero.
    /// </summary>
    protected virtual void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died!");
    }

    /// <summary>
    /// Returns current health value.
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Resets health to full.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = totalHealth;
        FireHealthEvent();
    }
}

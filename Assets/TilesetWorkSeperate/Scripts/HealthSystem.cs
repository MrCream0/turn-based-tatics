using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private Unit unit;
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private bool isEnemy;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        if (unit == null)
        {
            Debug.LogError($"Unit component is missing on {gameObject.name}!");
            return;
        }

        // Get UnitClass from Unit
        UnitClass unitClass = unit.GetUnitClass();
        if (unitClass == null)
        {
            Debug.LogError($"UnitClass is not assigned in Unit component on {gameObject.name}!");
            return;
        }

        //maxHealth = unitClass.baseMaxHealth;
        maxHealth = unitClass.baseMaxHealth * (1f + unitClass.healthGrowth * unit.GetLevel());
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (currentHealth == 0f && isEnemy)
        {
            Die();
            GameManager.Instance?.EndCombat(true);
        }
        else if (currentHealth == 0f && !isEnemy)
        {
            Die();
        }

        Debug.Log($"Current Health on {gameObject.name}: {currentHealth}");
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
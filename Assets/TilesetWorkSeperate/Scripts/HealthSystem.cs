using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField] private float currentHealth = 100.0f;
    [SerializeField] private float maxHealth;

    [SerializeField] private bool isEnemy;

    private void Awake()
    {
        maxHealth = currentHealth;
    }

    public void TakeDamage(float damageAmmount)
    {
        currentHealth -= damageAmmount;

        if (currentHealth <= 0.00f)
        {
            currentHealth = 0f;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (currentHealth == 0f && isEnemy)
        {
            Die();
            GameManager.Instance.EndCombat(true);
        }
        else if(currentHealth == 0f && !isEnemy)
        {
            Die();
        }

        Debug.Log("Current Health: " + currentHealth);

    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;//no need for casting since using float for health
    }
}

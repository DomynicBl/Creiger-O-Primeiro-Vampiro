using UnityEngine;
using UnityEngine.UI;
using System;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Image fillImage; // A imagem da barra de vida (preenchimento)

    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log($"{name} morreu!");
        OnDeath?.Invoke();
        // Aqui você pode animar a morte, destruir o inimigo, etc.
    }
}

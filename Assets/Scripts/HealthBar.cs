
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // Reference to the health bar image
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject shieldObject; // Reference to the shield GameObject

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Call this method to apply damage
    public void TakeDamage(float damage)
    {
        if (shieldObject == null || !shieldObject.activeSelf)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();
        }
        else
        {
            Debug.Log("Shield is active. Damage not applied.");
        }
    }

    // Call this method to heal
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // Update the health bar UI
    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
}
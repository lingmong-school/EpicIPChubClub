/*
* Author: Rayn Bin Kamaludin
* Date: 8/8/2024
* Description: Manages the player's health bar UI, applying damage and healing effects.
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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

            // Check if health has reached 0
            if (currentHealth <= 0)
            {
                OnDeath();
            }
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

    // Method called when health reaches 0
    private void OnDeath()
    {
        Debug.Log("Health reached 0. Changing to scene 4.");
        SceneManager.LoadScene(4); // Load scene 4
    }
}
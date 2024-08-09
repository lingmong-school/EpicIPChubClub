using UnityEngine;
using UnityEngine.UI;


public class BossHealthbar : MonoBehaviour
{
    public Image healthBarImage; // Reference to the health bar image
    public float maxHealth = 100f;
    private float currentHealth;

    public Transform playerCamera; // Reference to the player's camera

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        // Make the health bar face the player camera
        if (playerCamera != null)
        {
            transform.LookAt(transform.position + playerCamera.forward);
        }
    }

    // Call this method to apply damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    // Update the health bar UI
    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
}
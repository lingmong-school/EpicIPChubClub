using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the blocking ability for the player.
/// </summary>
public class Blocking : MonoBehaviour
{
    /// <summary>
    /// The GameObject to enable when blocking.
    /// </summary>
    public GameObject blockGameObject;

    private HealthBar healthBar; // Reference to the HealthBar script

    /// <summary>
    /// Disables the block GameObject on start.
    /// </summary>
    private void Start()
    {
        if (blockGameObject != null)
        {
            blockGameObject.SetActive(false);
        }

        // Find and store the HealthBar script
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            healthBar = player.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.shieldObject = blockGameObject; // Link the shield object to the HealthBar script
            }
        }
        else
        {
            Debug.LogError("Player object with HealthBar script not found. Ensure the player has a HealthBar script and is tagged as 'Player'.");
        }
    }

    /// <summary>
    /// Enables the block GameObject for a specified duration.
    /// </summary>
    /// <param name="duration">The duration to enable the GameObject.</param>
    public void EnableBlock(float duration)
    {
        StartCoroutine(BlockCoroutine(duration));
    }

    /// <summary>
    /// Coroutine to enable the block GameObject for a specified duration.
    /// </summary>
    /// <param name="duration">The duration to enable the GameObject.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator BlockCoroutine(float duration)
    {
        if (blockGameObject != null)
        {
            blockGameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            blockGameObject.SetActive(false);
        }
    }
}
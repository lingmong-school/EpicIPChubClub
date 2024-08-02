/*
 
Author: Rayn Kamaludin
Date: 2/8/2024
Description: Handles the calling of abilities such as the input and cooldown time for abilities like Dash and Sandy.
*/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Manages the abilities of the player, handling input and cooldown times.
/// </summary>
public class AbilityHandler : MonoBehaviour
{
    private PlayerControls controls;
    private bool canDash = true;
    private bool canSandy = true;
    public float dashCooldown = 5f;
    public float sandyCooldown = 10f;

    /// <summary>
    /// The UI image that indicates the cooldown progress for Dash.
    /// </summary>
    public Image dashCooldownIndicator;

    /// <summary>
    /// The UI image that indicates the cooldown progress for Sandy.
    /// </summary>
    public Image sandyCooldownIndicator;

    /// <summary>
    /// Initializes the player controls.
    /// </summary>
    private void Awake()
    {
        controls = new PlayerControls();
    }

    /// <summary>
    /// Enables the player controls and subscribes to the ability input actions.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Dash.performed += OnDashPerformed;
        controls.Player.Ultimate.performed += OnSandyPerformed;

        // Ensure the cooldown indicators start at full
        if (dashCooldownIndicator != null)
        {
            dashCooldownIndicator.fillAmount = 1f;
        }
        if (sandyCooldownIndicator != null)
        {
            sandyCooldownIndicator.fillAmount = 1f;
        }
    }

    /// <summary>
    /// Disables the player controls and unsubscribes from the ability input actions.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Dash.performed -= OnDashPerformed;
        controls.Player.Ultimate.performed -= OnSandyPerformed;
    }

    /// <summary>
    /// Called when the Dash input action is performed.
    /// Starts the Dash coroutine if the player can dash.
    /// </summary>
    /// <param name="context">The context of the input action.</param>
    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }

    /// <summary>
    /// Called when the Sandy input action is performed.
    /// Starts the Sandy coroutine if the player can use Sandy.
    /// </summary>
    /// <param name="context">The context of the input action.</param>
    private void OnSandyPerformed(InputAction.CallbackContext context)
    {
        if (canSandy)
        {
            StartCoroutine(Sandy());
        }
    }

    /// <summary>
    /// Handles the Dash ability, implementing the dash logic and cooldown.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator Dash()
    {
        Debug.Log("Dash performed!");

        // Implement dash logic here
        // For example, you can move the player forward quickly

        // Start cooldown
        canDash = false;
        if (dashCooldownIndicator != null)
        {
            dashCooldownIndicator.fillAmount = 0f;
        }

        float elapsedTime = 0f;
        while (elapsedTime < dashCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (dashCooldownIndicator != null)
            {
                dashCooldownIndicator.fillAmount = elapsedTime / dashCooldown;
            }
            yield return null;
        }

        canDash = true;
        if (dashCooldownIndicator != null)
        {
            dashCooldownIndicator.fillAmount = 1f;
        }

        Debug.Log("Dash cooldown finished, you can dash again.");
    }

    /// <summary>
    /// Handles the Sandy ability, implementing the Sandy logic and cooldown.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator Sandy()
    {
        Debug.Log("Sandy performed!");

        // Implement Sandy logic here
        // For example, you can apply an ultimate effect

        // Start cooldown
        canSandy = false;
        if (sandyCooldownIndicator != null)
        {
            sandyCooldownIndicator.fillAmount = 0f;
        }

        float elapsedTime = 0f;
        while (elapsedTime < sandyCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (sandyCooldownIndicator != null)
            {
                sandyCooldownIndicator.fillAmount = elapsedTime / sandyCooldown;
            }
            yield return null;
        }

        canSandy = true;
        if (sandyCooldownIndicator != null)
        {
            sandyCooldownIndicator.fillAmount = 1f;
        }

        Debug.Log("Sandy cooldown finished, you can use Sandy again.");
    }
}
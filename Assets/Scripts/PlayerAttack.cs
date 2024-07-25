/*
 
Author: Rayn Kamaludin
Date: 25/7/2024
Description: Player backstab enemy
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the player's attack input, allowing attacks only when the enemy can attack.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    public EnemyBack enemyBack; // Reference to the EnemyBack script
    public HiddenBlade hiddenBlade; // Reference to the HiddenBlade script
    private PlayerControls controls;
    private Animator animator;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Attack.performed += ctx => Attack();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    /// <summary>
    /// Handles the player's attack input.
    /// </summary>
    private void Attack()
    {
        if (enemyBack != null && enemyBack.canAttack)
        {
            // Activate the hidden blade
            hiddenBlade.ActivateBlade();

            // Set the Backstab animation boolean to true
            animator.SetBool("Backstab", true);
            Debug.Log("Player attacks with Backstab");
        }
        else
        {
            Debug.Log("Player cannot attack");
        }
    }

    /// <summary>
    /// Called when the Backstab animation ends.
    /// </summary>
    public void OnBackstabAnimationEnd()
    {
        // Deactivate the hidden blade
        hiddenBlade.DeactivateBlade();

        // Reset the Backstab animation boolean
        animator.SetBool("Backstab", false);
        Debug.Log("Backstab animation ended");
    }

    /// <summary>
    /// Called by the animation event to kill the enemy.
    /// </summary>
    public void Kill()
    {
        if (enemyBack != null && enemyBack.canAttack)
        {
            enemyBack.TriggerDeath();
        }
    }
}
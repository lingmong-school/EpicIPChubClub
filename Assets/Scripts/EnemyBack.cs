/*
 
Author: Rayn Kamaludin
Date: 25/7/2024
Description: Check for player within vicinity to attack 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the detection of the player within the enemy's trigger zone and the detection of the BackstabKnife.
/// </summary>
public class EnemyBack : MonoBehaviour
{
    [HideInInspector]
    public bool canAttack = false; // Boolean to check if the enemy can attack

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Detects when the player enters the trigger zone.
    /// </summary>
    /// <param name="other">The Collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = true;
        }
    }

    /// <summary>
    /// Detects when the player exits the trigger zone.
    /// </summary>
    /// <param name="other">The Collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = false;
        }
    }

    /// <summary>
    /// Triggers the death animation.
    /// </summary>
    public void TriggerDeath()
    {
        animator.SetBool("Dead", true);
    }
}
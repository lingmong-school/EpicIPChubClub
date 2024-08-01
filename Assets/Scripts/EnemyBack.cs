/*
Author: Rayn Kamaludin
Date: 25/7/2024
Description: Check for player within vicinity to attack 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX; // Add this namespace for Visual Effect Graph

/// <summary>
/// Handles the detection of the player within the enemy's trigger zone and the detection of the BackstabKnife.
/// </summary>
public class EnemyBack : MonoBehaviour
{
    [HideInInspector]
    public bool canAttack = false; // Boolean to check if the enemy can attack

    private Animator animator;
    public CapsuleCollider capsuleCollider;
    public EnemyBehavior enemyBehavior;
    public Canvas detectionUI;
    public BoxCollider boxCollider;

    [Header("Effects")]
    public ParticleSystem bloodSplatter; // Reference to the blood splatter particle system
    public VisualEffect visualEffect; // Reference to the Visual Effect component

    private void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
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
        else if (other.CompareTag("BackstabKnife") && canAttack)
        {
            TriggerDeath();
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
    /// Triggers the death animation and plays the blood splatter and visual effects.
    /// </summary>
    public void TriggerDeath()
    {
        animator.SetBool("Dead", true);
        capsuleCollider.enabled = false;
        enemyBehavior.enabled = false;
        detectionUI.enabled = false;
        boxCollider.enabled = false;

        if (bloodSplatter != null)
        {
            bloodSplatter.Play();
        }

        if (visualEffect != null)
        {
            visualEffect.Play();
        }

        Debug.Log("Enemy death triggered, collider disabled, and effects played");
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    public void FrontDeath()
    {
        animator.SetBool("Dead", true);
        capsuleCollider.enabled = false;
        enemyBehavior.enabled = false;
        detectionUI.enabled = false;
        boxCollider.enabled = false;

        if (bloodSplatter != null)
        {
            bloodSplatter.Play();
        }


        Debug.Log("Enemy death triggered, collider disabled, and effects played");
        StartCoroutine(DestroyAfterDelay());
    }
}
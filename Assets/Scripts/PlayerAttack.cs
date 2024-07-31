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

    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    // Cooldown time for AttackM1
    public float attackCooldown = 1f;
    private float lastAttackTime;

    // VFX
    public GameObject swing1VFX;
    public GameObject swing2VFX;
    public GameObject swing3VFX;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Click.performed += ctx => Attack();
        animator = GetComponent<Animator>();

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator does not have an AnimatorController assigned.");
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        ExitAttackM1();
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
            AttackM1();
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

    void AttackM1()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (Time.time - lastComboEnd > 0.2f && comboCounter < combo.Count)
            {
                CancelInvoke("EndCombo");

                if (Time.time - lastClickedTime >= 0.2f)
                {
                    // Activate the hidden blade at the start of the attack
                    hiddenBlade.ActivateBlade();

                    animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                    animator.Play("Attack", 0, 0);
                    // weapon.damage = combo[comboCounter].damage;
                    comboCounter++;
                    lastClickedTime = Time.time;
                    lastAttackTime = Time.time;

                    if (comboCounter >= combo.Count)
                    {
                        comboCounter = 0;
                    }
                }
            }
        }
    }

    void ExitAttackM1()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        // Deactivate the hidden blade after the combo ends
        hiddenBlade.DeactivateBlade();
    }

    // Animation event methods
    public void swing1()
    {
        PlayVFX(swing1VFX);
    }

    public void swing2()
    {
        PlayVFX(swing2VFX);
    }

    public void swing3()
    {
        PlayVFX(swing3VFX);
    }

    private void PlayVFX(GameObject vfx)
    {
        if (vfx != null)
        {
            Instantiate(vfx, transform.position, transform.rotation);
        }
    }
}
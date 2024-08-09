using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Climbing;

/// <summary>
/// Manages the player's attack input, allowing attacks only when the enemy can attack.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    public List<EnemyBack> enemyBackList = new List<EnemyBack>(); // Reference to the EnemyBack scripts
    public HiddenBlade hiddenBlade; // Reference to the HiddenBlade script
    private PlayerControls controls;
    private Animator animator;
    private MovementCharacterController movementCharacterController; // Reference to the MovementCharacterController

    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    // Cooldown time for AttackM1
    public float attackCooldown = 1f;
    private float lastAttackTime;

    // VFX
    public VisualEffect swing1VFX;
    public VisualEffect swing2VFX;
    public VisualEffect swing3VFX;

    // Audio Clips for each attack
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;
    public AudioClip attack3Sound;

    private AudioSource audioSource; // Reference to the AudioSource component
    private bool isAttacking = false; // Flag to indicate attack input

    public bool IsAttacking
    {
        get { return isAttacking; }
        private set { isAttacking = value; }
    }

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Attack.performed += ctx => Attack();
        controls.Player.Click.performed += ctx => AttackM1();
        animator = GetComponent<Animator>();
        movementCharacterController = GetComponent<MovementCharacterController>();

        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource reference
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure VFX components are assigned
        if (swing1VFX == null) Debug.LogError("swing1VFX is not assigned.");
        if (swing2VFX == null) Debug.LogError("swing2VFX is not assigned.");
        if (swing3VFX == null) Debug.LogError("swing3VFX is not assigned.");
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
        if (movementCharacterController.GetState() != MovementState.Walking && movementCharacterController.GetState() != MovementState.Running)
        {
            return; // Return if the player is not in walking or running state
        }

        foreach (var enemyBack in enemyBackList)
        {
            if (enemyBack != null && enemyBack.canAttack)
            {
                hiddenBlade.ActivateBlade();
                animator.SetBool("Backstab", true);
                IsAttacking = true;

                PlayAttackSound(attack1Sound); // Play the sound for Backstab

                Debug.Log("Player attacks with Backstab");
                return;
            }
        }
    }

    /// <summary>
    /// Called when the Backstab animation ends.
    /// </summary>
    public void OnBackstabAnimationEnd()
    {
        hiddenBlade.DeactivateBlade();
        animator.SetBool("Backstab", false);
        IsAttacking = false;

        Debug.Log("Backstab animation ended");
    }

    /// <summary>
    /// Called by the animation event to kill the enemy.
    /// </summary>
    public void Kill()
    {
        foreach (var enemyBack in enemyBackList)
        {
            if (enemyBack != null && enemyBack.canAttack)
            {
                enemyBack.TriggerDeath();
                return;
            }
        }
    }

    void AttackM1()
    {
        if (movementCharacterController.GetState() != MovementState.Walking && movementCharacterController.GetState() != MovementState.Running)
        {
            return; // Return if the player is not in walking or running state
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (Time.time - lastComboEnd > 0.2f && comboCounter < combo.Count)
            {
                CancelInvoke("EndCombo");

                if (Time.time - lastClickedTime >= 0.2f)
                {
                    hiddenBlade.ActivateBlade();

                    animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                    animator.Play("Attack", 0, 0);

                    // Play corresponding attack sound
                    switch (comboCounter)
                    {
                        case 0:
                            PlayAttackSound(attack1Sound);
                            break;
                        case 1:
                            PlayAttackSound(attack2Sound);
                            break;
                        case 2:
                            PlayAttackSound(attack3Sound);
                            break;
                    }

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
        hiddenBlade.DeactivateBlade();
    }

    // Animation event methods
    public void swing1()
    {
        PlayVFX(swing1VFX);
        Debug.Log("Swing1 VFX played");
    }

    public void swing2()
    {
        PlayVFX(swing2VFX);
        Debug.Log("Swing2 VFX played");
    }

    public void swing3()
    {
        PlayVFX(swing3VFX);
        Debug.Log("Swing3 VFX played");
    }

    private void PlayVFX(VisualEffect vfx)
    {
        if (vfx != null)
        {
            vfx.transform.position = transform.position + new Vector3(0, 1, 0);
            vfx.Play();
        }
    }

    private void PlayAttackSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemyBack = other.GetComponent<EnemyBack>();
            if (enemyBack != null && !enemyBackList.Contains(enemyBack))
            {
                enemyBackList.Add(enemyBack);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemyBack = other.GetComponent<EnemyBack>();
            if (enemyBack != null && enemyBackList.Contains(enemyBack))
            {
                enemyBackList.Remove(enemyBack);
            }
        }
    }
}
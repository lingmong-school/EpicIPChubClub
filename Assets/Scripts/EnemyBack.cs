using System.Collections;
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

    private PlayerAttack playerAttack; // Reference to the PlayerAttack script

    private void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();

        if (animator == null) Debug.LogError("Animator not assigned.");
        if (capsuleCollider == null) Debug.LogError("CapsuleCollider not assigned.");
        if (boxCollider == null) Debug.LogError("BoxCollider not assigned.");
        if (enemyBehavior == null) Debug.LogError("EnemyBehavior not assigned.");
        if (detectionUI == null) Debug.LogError("Detection UI not assigned.");
    }

    private void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        if (playerAttack == null) Debug.LogError("PlayerAttack script not found in the scene.");
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
        else if (other.CompareTag("BackstabKnife") && canAttack && playerAttack != null && playerAttack.IsAttacking)
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
    /// Triggers the death animation and plays the blood splatter and visual effects after a delay.
    /// </summary>
    public void TriggerDeath()
    {
        StartCoroutine(TriggerDeathAfterDelay(0.7f));
    }

    private IEnumerator TriggerDeathAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

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
        StartCoroutine(FrontDeathAfterDelay(0.5f));
    }

    private IEnumerator FrontDeathAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

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
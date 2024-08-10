using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; // Add this to handle scene switching

public class BossFight : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer;
    public float maxHealth = 100f; // Max health of the boss
    private float currentHealth; // Current health of the boss
    public float sightRange, attackRange;
    public float rechargeTime = 5f; // Time for recharge state
    public float redAttackCooldown = 2f; // Cooldown time for Red attack
    public float blueAttackCooldown = 1f; // Cooldown time for Blue attack
    public float purpleAttackCooldown = 1f; // Cooldown time for Purple attack

    private bool playerInSightRange, playerInAttackRange;
    private bool isSandyActive = false;

    private Animator animator; // Reference to the Animator
    private BossHealthbar bossHealthbar; // Reference to the BossHealthbar
    private BossAbility bossAbility; // Reference to the BossAbility
    private CapsuleCollider capsuleCollider; // Reference to the CapsuleCollider

    // Particle systems for abilities and recharge
    public ParticleSystem redAttackParticles;
    public GameObject blueAttackParticlePrefab;
    public ParticleSystem purpleAttackParticles;
    public GameObject rechargeParticleObject; // Reference to the GameObject containing the recharge particle system

    // Audio sources and clips for abilities
    private AudioSource audioSource; // AudioSource component
    public AudioClip redAttackSound; // Sound for Red attack
    public AudioClip blueAttackSound; // Sound for Blue attack
    public AudioClip purpleAttackSound; // Sound for Purple attack
    public AudioClip rechargeSound; // Sound for Recharge ability

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get the Animator component
        bossHealthbar = GetComponentInChildren<BossHealthbar>(); // Get the BossHealthbar component
        bossAbility = GetComponent<BossAbility>(); // Get the BossAbility component
        capsuleCollider = GetComponent<CapsuleCollider>(); // Get the CapsuleCollider component
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from the BossFight GameObject.");
        }
    }

    private void OnEnable()
    {
        AbilityHandler.OnSandyActivated += HandleSandyActivated;
        AbilityHandler.OnSandyDeactivated += HandleSandyDeactivated;
    }

    private void OnDisable()
    {
        AbilityHandler.OnSandyActivated -= HandleSandyActivated;
        AbilityHandler.OnSandyDeactivated -= HandleSandyDeactivated;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(StateMachine());

        // Ensure the recharge particle system is initially disabled
        if (rechargeParticleObject != null)
        {
            rechargeParticleObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Always face the player
        FacePlayer();
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (playerInSightRange && !playerInAttackRange)
            {
                yield return StartCoroutine(Chase());
            }
            else if (playerInAttackRange && playerInSightRange)
            {
                yield return StartCoroutine(AttackSequence());
            }
            else
            {
                animator.SetBool("Run", false);
                yield return null;
            }
        }
    }

    private IEnumerator Chase()
    {
        Debug.Log("Chasing Player");

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
        animator.SetBool("Run", true);

        yield return null;
    }

    private IEnumerator AttackSequence()
    {
        Debug.Log("Starting Attack Sequence");

        // Stop moving and look at the player
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position);
        }
        animator.SetBool("Run", false);

        // Red attack
        Debug.Log("Red Attack");
        animator.SetBool("Red", true);
        yield return new WaitForSeconds(isSandyActive ? 1.5f * 2 : 1.5f);
        animator.SetBool("Red", false);
        yield return new WaitForSeconds(isSandyActive ? redAttackCooldown * 2 : redAttackCooldown);

        // Check if player is still in attack range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInAttackRange) yield break;

        // Blue attack
        Debug.Log("Blue Attack");
        animator.SetBool("Blue", true);
        yield return new WaitForSeconds(isSandyActive ? 1f * 2 : 1f);
        animator.SetBool("Blue", false);
        yield return new WaitForSeconds(isSandyActive ? blueAttackCooldown * 2 : blueAttackCooldown);

        // Check if player is still in attack range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInAttackRange) yield break;

        // Purple attack
        Debug.Log("Purple Attack");
        animator.SetBool("Purple", true);
        yield return new WaitForSeconds(isSandyActive ? 1f * 2 : 1f);
        animator.SetBool("Purple", false);
        yield return new WaitForSeconds(isSandyActive ? purpleAttackCooldown * 2 : purpleAttackCooldown);

        // Recharge state
        Debug.Log("Recharging");
        animator.SetBool("Charge", true);
        if (rechargeParticleObject != null) rechargeParticleObject.SetActive(true); // Enable the recharge particle system
        yield return new WaitForSeconds(isSandyActive ? rechargeTime * 2 : rechargeTime);
        animator.SetBool("Charge", false);
        if (rechargeParticleObject != null) rechargeParticleObject.SetActive(false); // Disable the recharge particle system

        // Check if player is still in attack range after recharge
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (playerInAttackRange)
        {
            yield return StartCoroutine(AttackSequence());
        }
    }

    // Animation events
    public void RedPerformed()
    {
        if (redAttackParticles != null) redAttackParticles.Play();
        PlaySoundEffect(redAttackSound);
    }

    public void BluePerformed()
    {
        if (blueAttackParticlePrefab != null)
        {
            GameObject particleInstance = Instantiate(blueAttackParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particleInstance, 3f); // Destroy the particle after 3 seconds
        }
        PlaySoundEffect(blueAttackSound);
    }

    public void PurplePerformed()
    {
        if (purpleAttackParticles != null) purpleAttackParticles.Play();
        PlaySoundEffect(purpleAttackSound);
    }

    public void ChargePerformed()
    {
        if (rechargeParticleObject != null) rechargeParticleObject.SetActive(true); // Enable the recharge particle system
        PlaySoundEffect(rechargeSound);
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void FacePlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void HandleSandyActivated()
    {
        Debug.Log("Sandy activated, slowing down enemy behavior.");
        isSandyActive = true;
        agent.speed *= 0.5f; // Halve the agent speed
        bossAbility.SetSandyState(true); // Inform BossAbility about Sandy state
    }

    private void HandleSandyDeactivated()
    {
        Debug.Log("Sandy deactivated, restoring enemy behavior.");
        isSandyActive = false;
        agent.speed *= 2f; // Restore the agent speed
        bossAbility.SetSandyState(false); // Inform BossAbility about Sandy state
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Boss took {damage} damage. Remaining health: {currentHealth}");

        if (bossHealthbar != null)
        {
            bossHealthbar.TakeDamage(damage); // Update the health bar
        }

        if (currentHealth <= 0)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        animator.SetBool("Dead", true);
        agent.enabled = false;
        capsuleCollider.enabled = false;
        // Disable any other behaviors or components as needed
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(3); // Change to scene 3 after 2 seconds
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BackstabKnife"))
        {
            TakeDamage(10); // Adjust damage value as needed
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
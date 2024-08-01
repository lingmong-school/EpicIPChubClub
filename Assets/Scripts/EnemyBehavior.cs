
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    // Attention Level
    public float attentionTime = 5f; // Time in seconds to fully detect the player
    private float attentionLevel;
    private bool fullyDetected;
    private Coroutine attentionCoroutine;

    // UI Elements
    public GameObject attentionCanvas; // Reference to the canvas GameObject
    public Image attentionBar;
    private Camera playerCamera;

    // Damage delay
    private bool canTakeDamage = true;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        playerCamera = Camera.main;
        StartCoroutine(FOVRoutine());

        // Ensure the attention bar is set to 0 fill at the start and canvas is inactive
        if (attentionBar != null)
        {
            attentionBar.fillAmount = 0f;
        }

        if (attentionCanvas != null)
        {
            attentionCanvas.SetActive(false);
        }
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if (!canSeePlayer)
                    {
                        canSeePlayer = true;
                        if (attentionCoroutine == null)
                        {
                            attentionCoroutine = StartCoroutine(AttentionLevel());
                        }

                        if (attentionCanvas != null)
                        {
                            attentionCanvas.SetActive(true); // Activate the canvas
                        }
                    }
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }

        if (!canSeePlayer && attentionCoroutine != null)
        {
            StopCoroutine(attentionCoroutine);
            attentionCoroutine = null;
            attentionLevel = 0f; // Reset attention level if the player is not in sight
            if (attentionBar != null)
            {
                attentionBar.fillAmount = 0f; // Reset the attention bar
            }
            if (attentionCanvas != null)
            {
                attentionCanvas.SetActive(false); // Deactivate the canvas
            }
        }
    }

    private IEnumerator AttentionLevel()
    {
        while (attentionLevel < attentionTime)
        {
            attentionLevel += .1f; // Increment attention level every second (adjust as needed)
            UpdateAttentionBar();
            yield return new WaitForSeconds(.1f);
        }

        fullyDetected = true; // Player is fully detected
        Debug.Log("Player fully detected!");
    }

    private void UpdateAttentionBar()
    {
        if (attentionBar != null)
        {
            attentionBar.fillAmount = attentionLevel / attentionTime;
        }
    }

    private void LateUpdate()
    {
        if (attentionBar != null && playerCamera != null)
        {
            // Make the attention bar face the player's camera
            attentionBar.transform.LookAt(playerCamera.transform);
            attentionBar.transform.Rotate(0, 180, 0); // Adjust if the image is facing the wrong direction
        }
    }

    private void Awake()
    {
        // Debug log to check if the player object is found
        GameObject playerObj = GameObject.Find("PlayerObj");
        if (playerObj == null)
        {
            Debug.LogError("PlayerObj not found. Please ensure there is a GameObject named 'PlayerObj' in the scene.");
        }
        else
        {
            player = playerObj.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found. Please ensure the enemy GameObject has a NavMeshAgent component.");
        }
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!canSeePlayer && !playerInAttackRange) Patroling();
        if (canSeePlayer && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && canSeePlayer) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        Debug.Log("NPC found player");
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            /// Attack code here
            Debug.Log("NPC attack");
            /// End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            canTakeDamage = false;
            StartCoroutine(DamageCooldown(damage));
        }
    }

    private IEnumerator DamageCooldown(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Invoke(nameof(TriggerDeath), 0.5f); // Changed from DestroyEnemy to TriggerDeath
        }

        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }

    private void TriggerDeath()
    {
        var enemyBack = GetComponent<EnemyBack>();
        if (enemyBack != null)
        {
            enemyBack.FrontDeath();
        }
        else
        {
            Debug.LogError("EnemyBack component not found.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
    }
}
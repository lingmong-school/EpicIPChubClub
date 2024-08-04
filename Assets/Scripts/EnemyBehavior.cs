using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public float attentionTime = 5f;
    private float attentionLevel;
    private bool fullyDetected;
    private Coroutine attentionCoroutine;

    public GameObject attentionCanvas;
    public Image attentionBar;
    private Camera playerCamera;

    private bool canTakeDamage = true;
    private bool isSandyActive = false; // New flag to track if Sandy is active

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
        playerRef = GameObject.FindGameObjectWithTag("Player");
        playerCamera = Camera.main;
        StartCoroutine(FOVRoutine());

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
        if (isSandyActive)
        {
            canSeePlayer = false;
            if (attentionCoroutine != null)
            {
                StopCoroutine(attentionCoroutine);
                attentionCoroutine = null;
                attentionLevel = 0f;
                if (attentionBar != null)
                {
                    attentionBar.fillAmount = 0f;
                }
                if (attentionCanvas != null)
                {
                    attentionCanvas.SetActive(false);
                }
            }
            return;
        }

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
                            attentionCanvas.SetActive(true);
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
            attentionLevel = 0f;
            if (attentionBar != null)
            {
                attentionBar.fillAmount = 0f;
            }
            if (attentionCanvas != null)
            {
                attentionCanvas.SetActive(false);
            }
        }
    }

    private IEnumerator AttentionLevel()
    {
        while (attentionLevel < attentionTime)
        {
            attentionLevel += .1f;
            UpdateAttentionBar();
            yield return new WaitForSeconds(.1f);
        }

        fullyDetected = true;
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
            attentionBar.transform.LookAt(playerCamera.transform);
            attentionBar.transform.Rotate(0, 180, 0);
        }
    }

    private void Awake()
    {
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
        if (isSandyActive) return; // Skip the update if Sandy is active

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

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

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
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Debug.Log("NPC attack");

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
            Invoke(nameof(TriggerDeath), 0.5f);
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

    private void HandleSandyActivated()
    {
        Debug.Log("Sandy activated, slowing down enemy behavior.");
        isSandyActive = true;
        agent.speed *= 0.5f; // Halve the agent speed
        timeBetweenAttacks *= 2f; // Double the attack interval
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.speed *= 0.5f; // Halve the animation speed
        }
    }

    private void HandleSandyDeactivated()
    {
        Debug.Log("Sandy deactivated, restoring enemy behavior.");
        isSandyActive = false;
        agent.speed *= 2f; // Restore the agent speed
        timeBetweenAttacks /= 2f; // Restore the attack interval
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.speed *= 2f; // Restore the animation speed
        }
    }
}
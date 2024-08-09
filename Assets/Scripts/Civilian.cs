
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Civilian : MonoBehaviour
{
    public NavMeshAgent agent;
    public float walkPointRange;
    public float walkSpeed = 2f; // Speed at which the civilian walks
    private bool walkPointSet;
    private Vector3 walkPoint;

    private Animator animator; // Reference to the Animator

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    private void Start()
    {
        agent.speed = walkSpeed; // Set the walking speed
        StartCoroutine(StateMachine());
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            yield return StartCoroutine(Patroling());
        }
    }

    private IEnumerator Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        animator.SetBool("Walk", true); // Set Walk to true when patrolling

        yield return null;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, NavMesh.AllAreas))
            walkPointSet = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
    }
}
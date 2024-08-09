using System.Collections;
using UnityEngine;

public class BossAbility : MonoBehaviour
{
    public Transform redAbilityPoint;
    public Transform blueAbilityPoint;
    public Transform purpleAbilityPoint;
    public float redAbilityRange = 10f;
    public float blueAbilityRange = 15f;
    public float purpleAbilityRange = 20f;
    public float blueAbilityRadius = 2f; // Radius of the sphere for OverlapSphere
    public int redAbilityDamage = 10;
    public int blueAbilityDamage = 15;
    public int purpleAbilityDamage = 20;
    public float redAbilityDelay = 0.5f;
    public float blueAbilityDelay = 0.5f;
    public float purpleAbilityDelay = 0.5f;
    public LayerMask playerLayerMask;

    public ParticleSystem redAttackParticles; // Reference to red attack particle system
    public ParticleSystem purpleAttackParticles; // Reference to purple attack particle system

    private HealthBar playerHealthBar;
    private bool isSandyActive = false;

    private void Start()
    {
        // Find and store the player's HealthBar script
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealthBar = player.GetComponent<HealthBar>();
        }
        else
        {
            Debug.LogError("Player object with HealthBar script not found. Ensure the player has a HealthBar script and is tagged as 'Player'.");
        }
    }

    // Animation events
    public void RedPerformed()
    {
        StartCoroutine(PerformRedAbility());
    }

    public void BluePerformed()
    {
        StartCoroutine(PerformBlueAbility());
    }

    public void PurplePerformed()
    {
        StartCoroutine(PerformPurpleAbility());
    }

    private IEnumerator PerformRedAbility()
    {
        yield return new WaitForSeconds(isSandyActive ? redAbilityDelay * 2 : redAbilityDelay);

        RaycastHit hit;
        if (Physics.Raycast(redAbilityPoint.position, redAbilityPoint.forward, out hit, redAbilityRange, playerLayerMask))
        {
            Debug.Log("Red attack hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player") && playerHealthBar != null)
            {
                playerHealthBar.TakeDamage(redAbilityDamage);
            }
        }
    }

    private IEnumerator PerformBlueAbility()
    {
        yield return new WaitForSeconds(isSandyActive ? blueAbilityDelay * 2 : blueAbilityDelay);

        Collider[] hits = Physics.OverlapSphere(blueAbilityPoint.position, blueAbilityRadius, playerLayerMask);
        foreach (var hit in hits)
        {
            Debug.Log("Blue attack hit: " + hit.name);
            if (hit.CompareTag("Player") && playerHealthBar != null)
            {
                playerHealthBar.TakeDamage(blueAbilityDamage);
            }
        }
    }

    private IEnumerator PerformPurpleAbility()
    {
        yield return new WaitForSeconds(isSandyActive ? purpleAbilityDelay * 2 : purpleAbilityDelay);

        RaycastHit hit;
        if (Physics.Raycast(purpleAbilityPoint.position, purpleAbilityPoint.forward, out hit, purpleAbilityRange, playerLayerMask))
        {
            Debug.Log("Purple attack hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player") && playerHealthBar != null)
            {
                playerHealthBar.TakeDamage(purpleAbilityDamage);
            }
        }
    }

    public void SetSandyState(bool isActive)
    {
        isSandyActive = isActive;
        AdjustParticleSpeed(redAttackParticles);
        AdjustParticleSpeed(purpleAttackParticles);
        // Adjust other VFX as needed
    }

    private void AdjustParticleSpeed(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.simulationSpeed = isSandyActive ? 0.5f : 1f;
        }
    }

    // Gizmos to visualize ability ranges
    private void OnDrawGizmos()
    {
        if (redAbilityPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(redAbilityPoint.position, redAbilityPoint.forward * redAbilityRange);
        }

        if (blueAbilityPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(blueAbilityPoint.position, blueAbilityRadius);
        }

        if (purpleAbilityPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(purpleAbilityPoint.position, purpleAbilityPoint.forward * purpleAbilityRange);
        }
    }
}
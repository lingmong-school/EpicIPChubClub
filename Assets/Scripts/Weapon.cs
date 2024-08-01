using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float damage; // Made public

    BoxCollider triggerBox;

    // Start is called before the first frame update
    private void Start()
    {
        triggerBox = GetComponent<BoxCollider>();
        if (triggerBox == null)
        {
            Debug.LogError("TriggerBox is not assigned or found.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // Optionally, you can add a message indicating if the enemy would have died
                if (enemy.health <= 0)
                {
                    Debug.Log("Enemy would have died from this hit");

                    var capsuleCollider = other.GetComponent<CapsuleCollider>();
                    if (capsuleCollider != null)
                    {
                        capsuleCollider.enabled = false; // Disable the capsule collider
                    }
                    else
                    {
                        Debug.LogError("CapsuleCollider not found on the enemy object.");
                    }
                }
            }
            else
            {
                Debug.LogError("EnemyBehavior component not found on the collided object.");
            }
        }
    }
}
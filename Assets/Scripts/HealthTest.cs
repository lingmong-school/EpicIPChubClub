using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
* Author: Rayn Bin Kamaludin
* Date: 1/8/2024
* Description: Script to test health-related functionality.
*/


public class HealthTest : MonoBehaviour
{
    public float damageAmount = 1000f; // Amount of damage to apply to the player

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthBar healthBar = collision.gameObject.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(damageAmount);
            }
        }
    }
}
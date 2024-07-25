/*
 
Author: Rayn Kamaludin
Date: 25/7/2024
Description: Hidden blade handler
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the activation and deactivation of the hidden blade.
/// </summary>
public class HiddenBlade : MonoBehaviour
{
    private GameObject blade; // Reference to the blade GameObject

    private void Awake()
    {
        blade = this.gameObject;
        blade.SetActive(false); // Initially inactive
    }

    /// <summary>
    /// Activates the hidden blade.
    /// </summary>
    public void ActivateBlade()
    {
        blade.SetActive(true);
        Debug.Log("Hidden blade activated");
    }

    /// <summary>
    /// Deactivates the hidden blade.
    /// </summary>
    public void DeactivateBlade()
    {
        blade.SetActive(false);
        Debug.Log("Hidden blade deactivated");
    }
}
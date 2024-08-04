/*
 
Author: Rayn Kamaludin
Date: 2/8/2024
Description: Handles the blocking ability for the player.
*/

using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the blocking ability for the player.
/// </summary>
public class Blocking : MonoBehaviour
{
    /// <summary>
    /// The GameObject to enable when blocking.
    /// </summary>
    public GameObject blockGameObject;

    /// <summary>
    /// Disables the block GameObject on start.
    /// </summary>
    private void Start()
    {
        if (blockGameObject != null)
        {
            blockGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Enables the block GameObject for a specified duration.
    /// </summary>
    /// <param name="duration">The duration to enable the GameObject.</param>
    public void EnableBlock(float duration)
    {
        StartCoroutine(BlockCoroutine(duration));
    }

    /// <summary>
    /// Coroutine to enable the block GameObject for a specified duration.
    /// </summary>
    /// <param name="duration">The duration to enable the GameObject.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator BlockCoroutine(float duration)
    {
        blockGameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        blockGameObject.SetActive(false);
    }
}
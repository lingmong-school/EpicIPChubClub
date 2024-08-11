/*
* Author: Rayn Bin Kamaludin
* Date: 8/3/2024
* Description: Controls the camera during cutscenes, including transitioning between camera angles.
*/

using UnityEngine;

/// <summary>
/// Controls the camera during cutscenes, including transitioning between camera angles.
/// </summary>

public class CutsceneCamera : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found on the cutscene camera.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
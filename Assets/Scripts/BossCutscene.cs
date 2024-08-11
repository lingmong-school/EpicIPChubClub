/*
* Author: Rayn Bin Kamaludin
* Date: 8/7/2024
* Description: Controls the boss cutscene, including scene transitions and visual/audio effects.
*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this to handle scene switching
using UnityEngine.UI; // Add this to handle UI components like Image

/// <summary>
/// Controls the boss cutscene, including scene transitions and visual/audio effects.
/// </summary>

public class BossCutscene : MonoBehaviour
{
    private Animator animator;
    private bool cutsceneStarted = false;

    public GameObject cutsceneCamera; // Reference to the cutscene camera GameObject
    private Animator cameraAnimator; // Animator component of the cutscene camera

    public GameObject otherObjectWithDomain; // Reference to the other object with the Domain parameter
    private Animator otherAnimator; // Animator component of the other object

    public AudioClip cutsceneStartSound; // Sound for the start of the cutscene
    public AudioClip domainStartSound; // Sound for when "Domain" starts

    private AudioSource audioSource; // AudioSource component

    public float sceneSwitchDelay = 6f; // Time in seconds to wait before switching the scene
    public Animator sceneTransitionAnimator; // Reference to the Animator on the Canvas
    public Image transitionImage; // Reference to the Image component for the transition

    public BoxCollider boxColliderToDisable; // Reference to the BoxCollider to disable
    public GameObject meshToDisable; // Reference to the GameObject (Mesh) to disable

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogError("Animator not found on the GameObject.");
            return;
        }

        if (cutsceneCamera != null)
        {
            cameraAnimator = cutsceneCamera.GetComponent<Animator>();
            cutsceneCamera.SetActive(false); // Initially disable the cutscene camera
        }
        else
        {
            Debug.LogError("Cutscene camera is not assigned.");
        }

        if (otherObjectWithDomain != null)
        {
            otherAnimator = otherObjectWithDomain.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Other object with Domain parameter is not assigned.");
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (sceneTransitionAnimator == null)
        {
            Debug.LogError("Scene transition animator is not assigned.");
        }

        // Ensure the BoxCollider is enabled at the start
        if (boxColliderToDisable != null)
        {
            boxColliderToDisable.enabled = true;
        }

        // Ensure the Mesh (GameObject) is enabled at the start
        if (meshToDisable != null)
        {
            meshToDisable.SetActive(true);
        }

        // Disable the transition image at the start
        if (transitionImage != null)
        {
            transitionImage.enabled = false;
        }

        // Trigger the "End" animation at the start
        if (sceneTransitionAnimator != null)
        {
            sceneTransitionAnimator.SetTrigger("End");
        }
    }

    private IEnumerator CutsceneSequence()
    {
        // Play the cutscene start sound
        if (cutsceneStartSound != null)
        {
            audioSource.PlayOneShot(cutsceneStartSound);
        }

        // Enable the cutscene camera and set the "Cutscene" parameter to true
        if (cutsceneCamera != null)
        {
            cutsceneCamera.SetActive(true);
            if (cameraAnimator != null)
            {
                cameraAnimator.SetBool("Cutscene", true);
            }
        }

        // Set "talking" parameter to true
        animator.SetBool("Talking", true);
        Debug.Log("Talking started");

        // Wait for 10 seconds
        yield return new WaitForSeconds(10);

        // Set "talking" parameter to false
        animator.SetBool("Talking", false);
        Debug.Log("Talking ended");

        // Set "Domain" parameter to true in both animators
        animator.SetBool("Domain", true);
        if (otherAnimator != null)
        {
            otherAnimator.SetBool("Domain", true);
        }
        Debug.Log("Domain started");

        // Wait for 2 more seconds to complete 12 seconds
        yield return new WaitForSeconds(2);

        // Disable the cutscene camera
        if (cutsceneCamera != null)
        {
            cutsceneCamera.SetActive(false);
        }

        Debug.Log("Cutscene camera disabled");

        // Play the domain start sound right before triggering the scene transition
        if (domainStartSound != null)
        {
            audioSource.PlayOneShot(domainStartSound);
        }

        // Wait briefly to sync the sound with the animation trigger
        yield return new WaitForSeconds(0.5f);

        // Enable the transition image before triggering the transition animation
        if (transitionImage != null)
        {
            transitionImage.enabled = true;
        }

        // Trigger the scene transition animation
        if (sceneTransitionAnimator != null)
        {
            sceneTransitionAnimator.SetTrigger("Start");
        }

        // Wait for the transition animation duration
        yield return new WaitForSeconds(sceneSwitchDelay);

        // Load the new scene
        SceneManager.LoadScene(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cutsceneStarted)
        {
            cutsceneStarted = true;
            StartCoroutine(CutsceneSequence());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Trigger the end transition animation after the scene loads
        if (sceneTransitionAnimator != null)
        {
            sceneTransitionAnimator.SetTrigger("End");
            StartCoroutine(DisableAfterEndAnimation());
        }

        // Disable the specified BoxCollider and Mesh (GameObject) after the scene loads
        if (boxColliderToDisable != null)
        {
            boxColliderToDisable.enabled = false;
        }

        if (meshToDisable != null)
        {
            meshToDisable.SetActive(false);
        }
    }

    private IEnumerator DisableAfterEndAnimation()
    {
        // Wait until the end animation has finished
        AnimatorStateInfo stateInfo = sceneTransitionAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitUntil(() => stateInfo.IsName("End") && stateInfo.normalizedTime >= 1.0f);

        // Disable the GameObject after the animation ends
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
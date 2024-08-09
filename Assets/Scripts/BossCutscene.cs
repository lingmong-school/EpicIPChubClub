using System.Collections;
using UnityEngine;

public class BossCutscene : MonoBehaviour
{
    private Animator animator;
    private bool cutsceneStarted = false;

    public GameObject cutsceneCamera; // Reference to the cutscene camera GameObject
    private Animator cameraAnimator; // Animator component of the cutscene camera

    public GameObject otherObjectWithDomain; // Reference to the other object with the Domain parameter
    private Animator otherAnimator; // Animator component of the other object

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

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
    }

    private IEnumerator CutsceneSequence()
    {
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cutsceneStarted)
        {
            cutsceneStarted = true;
            StartCoroutine(CutsceneSequence());
        }
    }
}
using System.Collections;
using UnityEngine;

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
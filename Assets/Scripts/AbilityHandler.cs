using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Climbing;


public class AbilityHandler : MonoBehaviour
{
    private PlayerControls controls;
    private bool canBlock = true;
    private bool canSandy = true;
    public float blockCooldown = 5f;
    public float sandyCooldown = 10f;
    public float sandyDuration = 5f;

    public Image blockCooldownIndicator;
    public Image sandyCooldownIndicator;

    private Blocking blockingScript;
    private Sandevistan sandevistanScript;
    public MovementCharacterController movementCharacterController;
    public Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    public AudioClip blockSound;
    public AudioClip sandySound;
    private AudioSource audioSource;

    public static event System.Action OnSandyActivated;
    public static event System.Action OnSandyDeactivated;

    private void Awake()
    {
        controls = new PlayerControls();
        blockingScript = GetComponent<Blocking>();
        sandevistanScript = GetComponent<Sandevistan>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.Log("Color Adjustments found in the Volume profile.");
        }
        else
        {
            Debug.LogError("Color Adjustments not found in the Volume profile.");
        }

        if (globalVolume.profile.TryGet<Vignette>(out vignette))
        {
            Debug.Log("Vignette found in the Volume profile.");
        }
        else
        {
            Debug.LogError("Vignette not found in the Volume profile.");
        }
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Block.performed += OnBlockPerformed;
        controls.Player.Ultimate.performed += OnSandyPerformed;

        if (blockCooldownIndicator != null)
        {
            blockCooldownIndicator.fillAmount = 1f;
        }
        if (sandyCooldownIndicator != null)
        {
            sandyCooldownIndicator.fillAmount = 1f;
        }
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Block.performed -= OnBlockPerformed;
        controls.Player.Ultimate.performed -= OnSandyPerformed;
    }

    private void OnBlockPerformed(InputAction.CallbackContext context)
    {
        if (canBlock)
        {
            StartCoroutine(Block());
        }
    }

    private void OnSandyPerformed(InputAction.CallbackContext context)
    {
        if (canSandy)
        {
            StartCoroutine(Sandy());
        }
    }

    private IEnumerator Block()
    {
        Debug.Log("Block performed!");
        blockingScript.EnableBlock(1f);

        // Play block sound effect
        if (blockSound != null)
        {
            audioSource.PlayOneShot(blockSound);
        }

        canBlock = false;
        if (blockCooldownIndicator != null)
        {
            blockCooldownIndicator.fillAmount = 0f;
        }

        float elapsedTime = 0f;
        while (elapsedTime < blockCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (blockCooldownIndicator != null)
            {
                blockCooldownIndicator.fillAmount = elapsedTime / blockCooldown;
            }
            yield return null;
        }

        canBlock = true;
        if (blockCooldownIndicator != null)
        {
            blockCooldownIndicator.fillAmount = 1f;
        }

        Debug.Log("Block cooldown finished, you can block again.");
    }

    private IEnumerator Sandy()
    {
        Debug.Log("Sandy performed!");

        sandevistanScript.ActivateSandy();
        movementCharacterController.SetCurrentState(MovementState.Walking);
        OnSandyActivated?.Invoke();

        // Play Sandevistan sound effect
        if (sandySound != null)
        {
            audioSource.PlayOneShot(sandySound);
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = new Color(121f / 255f, 159f / 255f, 129f / 255f);
        }

        if (vignette != null)
        {
            StartCoroutine(AdjustVignette(vignette, 0f, 0.43f, sandyDuration));
        }

        float originalRunSpeed = movementCharacterController.RunSpeed;
        movementCharacterController.RunSpeed = 9f;

        yield return new WaitForSeconds(sandyDuration);

        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = Color.white;
        }

        if (vignette != null)
        {
            StartCoroutine(AdjustVignette(vignette, 0.43f, 0f, 0.1f));
        }

        movementCharacterController.RunSpeed = originalRunSpeed;
        movementCharacterController.SetCurrentState(MovementState.Walking);

        OnSandyDeactivated?.Invoke();

        canSandy = false;
        if (sandyCooldownIndicator != null)
        {
            sandyCooldownIndicator.fillAmount = 0f;
        }

        float elapsedTime = 0f;
        while (elapsedTime < sandyCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (sandyCooldownIndicator != null)
            {
                sandyCooldownIndicator.fillAmount = elapsedTime / sandyCooldown;
            }
            yield return null;
        }

        canSandy = true;
        if (sandyCooldownIndicator != null)
        {
            sandyCooldownIndicator.fillAmount = 1f;
        }

        Debug.Log("Sandy cooldown finished, you can use Sandy again.");
    }

    private IEnumerator AdjustVignette(Vignette vignette, float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            vignette.intensity.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.value = endValue;
    }
}
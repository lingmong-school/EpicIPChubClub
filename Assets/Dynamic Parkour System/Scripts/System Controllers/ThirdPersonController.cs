using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
    [RequireComponent(typeof(InputCharacterController))]
    [RequireComponent(typeof(MovementCharacterController))]
    [RequireComponent(typeof(AnimationCharacterController))]
    [RequireComponent(typeof(DetectionCharacterController))]
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(VaultingController))]

    public class ThirdPersonController : MonoBehaviour
    {
        [HideInInspector] public InputCharacterController characterInput;
        [HideInInspector] public MovementCharacterController characterMovement;
        [HideInInspector] public AnimationCharacterController characterAnimation;
        [HideInInspector] public DetectionCharacterController characterDetection;
        [HideInInspector] public VaultingController vaultingController;
        [HideInInspector] public bool isGrounded = false;
        [HideInInspector] public bool allowMovement = true;
        [HideInInspector] public bool onAir = false;
        [HideInInspector] public bool isJumping = false;
        [HideInInspector] public bool inSlope = false;
        [HideInInspector] public bool isVaulting = false;
        [HideInInspector] public bool dummy = false;
        [HideInInspector] public bool isCrouching = false; // coded by Rayn

        [Header("Cameras")]
        public CameraController cameraController;
        public Transform mainCamera;
        public Transform freeCamera;

        [Header("Step Settings")]
        [Range(0, 10.0f)] public float stepHeight = 0.8f;
        public float stepVelocity = 0.2f;

        [Header("Colliders")]
        public CapsuleCollider normalCapsuleCollider;
        public CapsuleCollider slidingCapsuleCollider;
        public float crouchHeight = 1.0f; // coded by Rayn

        private float turnSmoothTime = 0.1f;
        private float turnSmoothVelocity;
        private float originalHeight;
        private float originalSpeed;

        private void Awake()
        {
            characterInput = GetComponent<InputCharacterController>();
            characterMovement = GetComponent<MovementCharacterController>();
            characterAnimation = GetComponent<AnimationCharacterController>();
            characterDetection = GetComponent<DetectionCharacterController>();
            vaultingController = GetComponent<VaultingController>();

            originalHeight = normalCapsuleCollider.height;
            originalSpeed = characterMovement.walkSpeed;

            if (cameraController == null)
                UnityEngine.Debug.LogError("Attach the Camera Controller located in the Free Look Camera");
        }

        private void Start()
        {
            characterMovement.OnLanded += characterAnimation.Land;
            characterMovement.OnFall += characterAnimation.Fall;
        }

        void Update()
        {
            // Detect if Player is on Ground
            isGrounded = OnGround();

            // Get Input if controller and movement are not disabled
            if (!dummy && allowMovement)
            {
                AddMovementInput(characterInput.movement);

                // Detects if Joystick is being pushed hard
                if (characterInput.run && characterInput.movement.magnitude > 0.5f && !isCrouching)
                {
                    ToggleRun();
                }
                else if (!characterInput.run || isCrouching)
                {
                    ToggleWalk();
                }

                // Handle crouching input (coded by Rayn)
                HandleCrouch();
            }
        }

        /// <summary>
        /// Detects if the player is on the ground.
        /// </summary>
        /// <returns>True if the player is on the ground, false otherwise.</returns>
        private bool OnGround()
        {
            return characterDetection.IsGrounded(stepHeight);
        }

        /// <summary>
        /// Adds movement input to the character.
        /// </summary>
        /// <param name="direction">The direction of movement input.</param>
        public void AddMovementInput(Vector2 direction)
        {
            Vector3 translation = Vector3.zero;
            translation = GroundMovement(direction);
            characterMovement.SetVelocity(Vector3.ClampMagnitude(translation, 1.0f));
        }

        /// <summary>
        /// Calculates ground movement based on input.
        /// </summary>
        /// <param name="input">The input direction.</param>
        /// <returns>The calculated movement vector.</returns>
        Vector3 GroundMovement(Vector2 input)
        {
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            // Gets direction of movement relative to the camera rotation
            freeCamera.eulerAngles = new Vector3(0, mainCamera.eulerAngles.y, 0);
            Vector3 translation = freeCamera.transform.forward * input.y + freeCamera.transform.right * input.x;
            translation.y = 0;

            // Detects if player is moving in any direction
            if (translation.magnitude > 0)
            {
                RotatePlayer(direction);
                characterAnimation.animator.SetBool("Released", false);
            }
            else
            {
                ToggleWalk();
                characterAnimation.animator.SetBool("Released", true);
            }

            return translation;
        }

        /// <summary>
        /// Rotates the player to face the given direction.
        /// </summary>
        /// <param name="direction">The direction to face.</param>
        public void RotatePlayer(Vector3 direction)
        {
            // Get direction with camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            // Rotate Mesh to Movement
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        /// <summary>
        /// Rotates the player to face the camera direction.
        /// </summary>
        /// <param name="direction">The direction to face.</param>
        /// <returns>The calculated rotation.</returns>
        public Quaternion RotateToCameraDirection(Vector3 direction)
        {
            // Get direction with camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            // Rotate Mesh to Movement
            return Quaternion.Euler(0f, targetAngle, 0f);
        }

        /// <summary>
        /// Resets the movement of the character.
        /// </summary>
        public void ResetMovement()
        {
            characterMovement.ResetSpeed();
        }

        /// <summary>
        /// Toggles the character's running state.
        /// </summary>
        public void ToggleRun()
        {
            if (characterMovement.GetState() != MovementState.Running)
            {
                characterMovement.SetCurrentState(MovementState.Running);
                characterMovement.curSpeed = characterMovement.RunSpeed;
                characterAnimation.animator.SetBool("Run", true);
            }
        }

        /// <summary>
        /// Toggles the character's walking state.
        /// </summary>
        public void ToggleWalk()
        {
            if (characterMovement.GetState() != MovementState.Walking)
            {
                characterMovement.SetCurrentState(MovementState.Walking);
                characterMovement.curSpeed = isCrouching ? originalSpeed / 1.2f : characterMovement.walkSpeed; // Halve speed if crouching (coded by Rayn)
                characterAnimation.animator.SetBool("Run", false);
            }
        }

        /// <summary>
        /// Handles the character's crouching state. (coded by Rayn)
        /// </summary>
        public void HandleCrouch()
        {
            if (characterInput.crouch)
            {
                if (!isCrouching)
                {
                    isCrouching = true;
                    characterAnimation.animator.SetBool("Crouch", true);
                    normalCapsuleCollider.height = crouchHeight;
                    characterMovement.curSpeed = originalSpeed / 1.2f; // Reduce speed to half when crouching
                }
            }
            else
            {
                if (isCrouching)
                {
                    isCrouching = false;
                    characterAnimation.animator.SetBool("Crouch", false);
                    normalCapsuleCollider.height = originalHeight;
                    characterMovement.curSpeed = originalSpeed; // Restore original speed when not crouching
                }
            }
        }

        /// <summary>
        /// Gets the current velocity of the character.
        /// </summary>
        /// <returns>The current velocity.</returns>
        public float GetCurrentVelocity()
        {
            return characterMovement.GetVelocity().magnitude;
        }

        /// <summary>
        /// Disables the character controller.
        /// </summary>
        public void DisableController()
        {
            characterMovement.SetKinematic(true);
            characterMovement.enableFeetIK = false;
            dummy = true;
            allowMovement = false;
        }

        /// <summary>
        /// Enables the character controller.
        /// </summary>
        public void EnableController()
        {
            characterMovement.SetKinematic(false);
            characterMovement.EnableFeetIK();
            characterMovement.ApplyGravity();
            characterMovement.stopMotion = false;
            dummy = false;
            allowMovement = true;
        }
    }
}
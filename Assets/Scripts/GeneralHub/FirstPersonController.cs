using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class FirstPersonController : MonoBehaviour
{

    // Variable to make sure that the user is able to move:
    public bool CanMove { get; private set; } = true;
    // Checking if the player is sprinting or not
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    // Checking if the player is able to jump (On ground and pressing space)
    private bool ShouldJump => Input.GetKey(jumpKey) && characterController.isGrounded;
    // Checking if the player is crouching
    private bool IsCrouching => (canCrouch && Input.GetKey(crouchKey));
    // Checking if the player is able to crouch
    private bool ShouldCrouch => Input.GetKey(crouchKey) && characterController.isGrounded && !duringCrouchAnmation;

    // Setting up the movement parameters:
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8.0f;

    // Setting up the player's control on their field of view
    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f; // mouse sensitivity to look around in the x direction
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f; // mouse sensitivity to look around in the y direction
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f; // How much we can look up
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f; // How much we can look down

    // Just to help us choose between the features we want
    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool WillSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootsteps = false;

    // Mapping Keyboard buttons to player functionality
    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode zoomkey = KeyCode.Mouse2;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // Setting up player's jumping
    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    // Setting up a crouching mechanic
    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f; // Crouch Height
    [SerializeField] private float standingHeight = 2.0f; // Stand Height
    [SerializeField] private float timeToCrouch = 0.25f; // How long it takes to crouch and stand
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0); // center of the character when they're crouching
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0); // center of the character when they're standing
    private bool isCrouching;
    private bool duringCrouchAnmation; // Moving from crouching to standing
    private GameObject playerModel;
    private GameObject playerControllerModel;

    // Setting up the headbob (Was in the tutorial I followed and it adds some immersive factor to the movement)
    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f; // Speed of the bob while walking
    [SerializeField] private float walkBobAmount = 0.05f; // Intensity of the bob while walking
    [SerializeField] private float sprintBobSpeed = 18f; // Speed of the bob while sprinting
    [SerializeField] private float sprintBobAmount = 0.11f; // Intensity of the bob while sprinting
    [SerializeField] private float crouchBobSpeed = 8f; // Speed of the bob while crouching
    [SerializeField] private float crouchBobAmount = 0.025f; // Intensity of the bob while crouching
    private float defaultYPos = 0; // The vertical camera position
    private float timer;

    // Setting up sliding parameters
    private Vector3 hitPointNormal;
    private bool IsSliding
    {
        get
        {
            // Check if the player is sliding on a steep slope
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 1.5f))
            {
                hitPointNormal = slopeHit.normal; // Get the slope's normal vector

                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit; // the slope steepness
            }
            else
            {
                return false;
            }
        }
    }

    // Setting up player Zooming
    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    // Setting up player interaction
    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    // Object interactions:
    // Add these variables to your class
    [Header("Object Interaction")]
    [SerializeField] private Transform carryPoint; // Position where the object is held
    [SerializeField] private float throwForce = 10f; // Force applied to throw the object
    private GameObject carriedObject; // Reference to the currently carried object
    private Rigidbody carriedObjectRb; // Rigidbody of the carried object

    // Setting up player footsteps
    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    private float footstepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler : IsSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

    //// Setting up Animations
    [Header("Player Animations")]
    private Animator playerAnimation;


    private Camera playerCamera;
    private Camera cameraRoot;
    private CharacterController characterController;

    private Vector3 moveDirection; // The direction we're moving in
    private Vector2 currentInput; // Keyboard input for the movement

    private float rotationX = 0;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        //playerAnimation = GetComponentInChildren<CharacterAnimation>();
        playerAnimation = GetComponentInChildren<Animator>();
        playerModel = GameObject.Find("FirstPersonController/PlayerModel");
        playerControllerModel = GameObject.Find("FirstPersonController");
        defaultYPos = playerCamera.transform.localPosition.y; // This is for the headbob
        defaultFOV = playerCamera.fieldOfView;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
            {
                HandleJump();
            }

            if (canCrouch)
            {
                HandleCrouch();
            }
            if (canUseHeadbob)
            {
                HandleHeadBob();
            }
            if (canZoom)
            {
                HandleZoom();
            }
            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }
            if (useFootsteps)
            {
                Handle_Footsteps();
            }
            ApplyFinalMovement();

            //HandleAnimations();
            HandleObjectInteraction();

        }
        // Calculate Speed
        float speed = characterController.velocity.magnitude;
        //print("Speed Is: " + speed);
        playerAnimation.SetFloat("Speed", speed);

        // Update Grounded Status
        playerAnimation.SetBool("IsGrounded", characterController.isGrounded);

        // Update Crouching Status
        playerAnimation.SetBool("IsCrouching", isCrouching);

        // Vertical Velocity for Jump/Fall
        playerAnimation.SetFloat("VerticalVelocity", characterController.velocity.y);
    }

    // This function will map the keyboard keys to the actual movement functions that we need to link them to
    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        if (IsSprinting)
        {
            moveDirection = moveDirection.normalized * Mathf.Clamp(moveDirection.magnitude, 0, sprintSpeed);
        }
        else if (IsCrouching)
        {
            moveDirection = moveDirection.normalized * Mathf.Clamp(moveDirection.magnitude, 0, crouchSpeed);
        }
        else
        {
            moveDirection = moveDirection.normalized * Mathf.Clamp(moveDirection.magnitude, 0, walkSpeed);
        }
        moveDirection.y = moveDirectionY;

    }

    private void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand()); // change between standing and crouching
        }

    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void HandleZoom()
    {
        if (Input.GetKeyDown(zoomkey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }
        if (Input.GetKeyUp(zoomkey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }

    }

    // Mapping the mouse movement to player's rotation
    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 6 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);
                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }
    private void HandleObjectInteraction()
    {
        if (Input.GetKeyDown(interactKey)) // E key pressed
        {
            if (carriedObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                DropObject();
            }
        }

        if (Input.GetMouseButtonDown(0) && carriedObject != null) // Left mouse click
        {
            ThrowObject();
        }
    }

    // Try to pick up an object
    private void TryPickUpObject()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            if (hit.collider.CompareTag("Interactable/Pickup")) // Ensure the object has the "Pickup" tag
            {
                carriedObject = hit.collider.gameObject;
                carriedObjectRb = carriedObject.GetComponent<Rigidbody>();

                if (carriedObjectRb != null)
                {
                    carriedObjectRb.isKinematic = true; // Disable physics while carrying
                    carriedObject.transform.position = carryPoint.position;
                    carriedObject.transform.rotation = carryPoint.rotation;
                    carriedObject.transform.SetParent(carryPoint); // Attach to carry point
                }
            }
        }
    }

    // Drop the currently carried object
    private void DropObject()
    {
        if (carriedObjectRb != null)
        {
            carriedObjectRb.isKinematic = false; // Enable physics again
        }
        carriedObject.transform.SetParent(null); // Detach from carry point
        carriedObject = null;
        carriedObjectRb = null;
    }

    // Throw the currently carried object
    private void ThrowObject()
    {
        if (carriedObjectRb != null)
        {
            carriedObjectRb.isKinematic = false; // Enable physics
            carriedObject.transform.SetParent(null); // Detach from carry point
            carriedObjectRb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse); // Apply throw force
            carriedObject = null;
            carriedObjectRb = null;
        }
    }


    private void Handle_Footsteps()
    {
        if (!characterController.isGrounded)
        {
            return;
        }
        if (currentInput == Vector2.zero)
        {
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/WOOD":
                        footstepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                        break;
                    case "Footsteps/METAL":
                        footstepAudioSource.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                        break;
                    case "Footsteps/GRASS":
                        footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                }
            }
            footstepTimer = GetCurrentOffset;
        }
    }

    //// Going back to the idle state
    //private void ResetAnimations()
    //{
    //    playerAnimation.Walk(false);
    //    playerAnimation.Running(false);
    //    playerAnimation.Crouching(false);
    //    playerAnimation.Crouch_Walking(false);
    //    playerAnimation.Sliding(false);
    //    playerAnimation.IdleAnimation(true);
    //}

    //// Organizing the Animation conditions for the player's movement
    //private void HandleAnimations()
    //{

    //    if (IsCrouching && playerAnimation.GetIdleState() == true)
    //    {
    //        playerAnimation.IdleAnimation(false);
    //        playerAnimation.Crouching(true);
    //        if (currentInput != Vector2.zero)
    //        {
    //            playerAnimation.Crouch_Walking(true);
    //        }
    //    }
    //    else if (IsSprinting)
    //    {
    //        playerAnimation.Running(true);
    //    }
    //    else if (currentInput != Vector2.zero && playerAnimation.GetIdleState() == true)
    //    {
    //        playerAnimation.IdleAnimation(false);
    //        playerAnimation.Walk(true);
    //    }
    //    else if (IsSliding)
    //    {
    //        playerAnimation.Sliding(true);
    //    }
    //    else
    //    {
    //            ResetAnimations();
    //    }
    //}

    private void ApplyFinalMovement()
    {
        //if(characterController.velocity.y < -1 && characterController.isGrounded)
        //{
        //    moveDirection.y = 0;
        //}

        if (!characterController.isGrounded) // Handling gravity
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (WillSlideOnSlopes && IsSliding) // Sliding on the slope
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
            //moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * Mathf.Clamp(Vector3.Angle(hitPointNormal, Vector3.up) / characterController.slopeLimit, 1, 2) * slopeSpeed;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // To transition between crouching nad standing
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }
        duringCrouchAnmation = true;
        //if (IsCrouching && Input.GetKeyDown(crouchKey) == true)
        //{
        //    //playerAnimation.Crouching(true);
        //playerModel.transform.position = new Vector3(characterController.transform.position.x, characterController.transform.position.y + 0.48f, characterController.transform.position.z);
        //}
        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        // This is to ensure that the player is changing from crouching to standing smoother
        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnmation = false;
        //if ((isCrouching == false && duringCrouchAnmation == false) && Input.GetKeyDown(crouchKey) == false)
        //{
        //    //playerAnimation.Crouching(false);
        //playerModel.transform.position = new Vector3(characterController.transform.position.x, characterController.transform.position.y - 0.48f, characterController.transform.position.z);
        //}
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
    //    // Returns the position of the character
    //    public Vector3 GetCharacterPosition()
    //    {
    //        float targetFOV = isEnter ? zoomFOV : defaultFOV;
    //        float startingFOV = playerCamera.fieldOfView;
    //        float timeElapsed = 0;

    //        while (timeElapsed < timeToZoom)
    //        {
    //            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
    //            timeElapsed += Time.deltaTime;
    //            yield return null;
    //        }
    //        playerCamera.fieldOfView = targetFOV;
    //        zoomRoutine = null;
    //    }
    public Vector3 GetCharacterPosition()
    {
        return characterController.transform.position;
    }
    public void SetCharacterSpeed(float newWalkSpeed, float newSprintSpeed)
    {
        walkSpeed = newWalkSpeed;
        sprintSpeed = newSprintSpeed;
    }

    public void SetJumpHeight(float newJumpHeight)
    {
        jumpForce = newJumpHeight;
    }

    // Checks for collisions with the player
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Enemy"))
        {
            
        }
    }

    //  Check for collision with rocks
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            Debug.Log("Rock and Person Collided");
        }
    }
}



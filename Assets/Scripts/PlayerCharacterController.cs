using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    private const float NORMAL_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;
    public float moveSpeed;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float reachedHookshotPositionDistance;
    [SerializeField] private float hookshotThrowSpeed;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform hookshotTransform;

    private CharacterController characterController;
    private CameraFov cameraFov;
    private float cameraVerticalAngle;
    private float characterVelocityY;
    private float hookshotSize;
    private Vector3 characterVelocityMomentum;

    private Vector3 hookshotPosition;
    private State state;

    private enum State
    {
        Normal,
        HookShotFlyingPlayer,
        HookshotThrown,
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cameraFov = playerCamera.GetComponent<CameraFov>();
        state = State.Normal;
        hookshotTransform.gameObject.SetActive(false);
    }

    // activates the cursor in teleport mode
    void OnGUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.Normal:
                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHookshotStart();
                break;
            case State.HookshotThrown:

                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHookshotThrown();
                break;
            case State.HookShotFlyingPlayer:
                HandleCharacterLook();
                HandleHookShotMovement();
                break;
        }
    }

    private void HandleCharacterLook()
    {
        float lookX = Input.GetAxisRaw("Mouse X");
        float lookY = Input.GetAxisRaw("Mouse Y");

        // Rotate the transform with the input speed around its local Y axis
        transform.Rotate(new Vector3(0f, lookX * mouseSensitivity, 0f), Space.Self);

        // Add vertical inputs to the camera's vertical angle
        cameraVerticalAngle -= lookY * mouseSensitivity;

        // Apply the vertical angle as a local rotation to the camera transform along its right axis
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }

    private void HandleCharacterMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 characterVelocity = transform.right * moveX * moveSpeed + transform.forward * moveZ * moveSpeed;

        if (characterController.isGrounded)
        {
            characterVelocityY = 0f;
            // Jump
            if (TestInputJumpDown())
            {
                float jumpSpeed = 30f;
                characterVelocityY = jumpSpeed;
            }
        }

        // Apply gravity to the velocity
        float gravityDownForce = -60f;
        characterVelocityY += gravityDownForce * Time.deltaTime;

        // Apply Y velocity to move vector
        characterVelocity.y = characterVelocityY;

        // Apply momentum
        characterVelocity += characterVelocityMomentum;

        // Move Character Controller
        characterController.Move(characterVelocity * Time.deltaTime);

        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }
        }
    }

    private void resetGravityEffect()
    {
        characterVelocityY = 0f;
    }

    private void HandleHookshotStart()
    {
        if (TestInputHookshotDown())
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit))
            {
                //Hit something
                print("Hookshot!");
                hookshotPosition = raycastHit.point;
                hookshotSize = 0f;
                hookshotTransform.gameObject.SetActive(true);
                hookshotTransform.localScale = Vector3.zero;
                state = State.HookshotThrown;
            }
        }
    }

    private void HandleHookshotThrown()
    {
        hookshotTransform.LookAt(hookshotPosition);
        hookshotSize += hookshotThrowSpeed * Time.deltaTime;
        hookshotTransform.localScale = new Vector3(1, 1, hookshotSize);

        if(hookshotSize >= Vector3.Distance(transform.position, hookshotPosition))
        {
            state = State.HookShotFlyingPlayer;
            cameraFov.SetCameraFov(HOOKSHOT_FOV);
        }
    }

    private void HandleHookShotMovement()
    {
        hookshotTransform.LookAt(hookshotPosition);
        Vector3 hookShotDir = (hookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 15f;
        float hookshotSpeedMax = 50f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;
        // move Character Controller
        characterController.Move(hookShotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance)
        {
            // reached hookshot position
            StopHookshot();
        }
        if (TestInputHookshotDown())
        {
            // canceled hookshot
            StopHookshot();
        }
        if (TestInputJumpDown())
        {
            // cancelled with jump
            float momentumExtraSpeed = 7f;
            characterVelocityMomentum = hookShotDir * hookshotSpeed * momentumExtraSpeed;
            float jumpSpeed = 10f;
            characterVelocityMomentum += Vector3.up * jumpSpeed;
            StopHookshot();
        }
    }

    private void StopHookshot()
    {
        state = State.Normal;
        resetGravityEffect();
        hookshotTransform.gameObject.SetActive(false);
        cameraFov.SetCameraFov(NORMAL_FOV);
    }

    private bool TestInputHookshotDown()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    private bool TestInputJumpDown()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}

using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float crouchSpeed = 2f;
    public float jumpForce = 6f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    private float verticalRotation = 0f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;

    private StaminaSystem stamina;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            enabled = false;
            return;
        }

        controller = GetComponent<CharacterController>();
        stamina = GetComponent<StaminaSystem>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!IsOwner) return;

        Look();
        Move();
    }

    void Move()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
        bool canRun = stamina != null && stamina.CanRun();
        bool running = wantsToRun && canRun;

        stamina?.SetRunning(running);

        bool wantsToCrouch = Input.GetKey(KeyCode.LeftControl);
        if (wantsToCrouch && !isCrouching)
        {
            controller.height = crouchHeight;
            isCrouching = true;
        }
        else if (!wantsToCrouch && isCrouching)
        {
            controller.height = standHeight;
            isCrouching = false;
        }

        float currentSpeed = isCrouching ? crouchSpeed : (running ? runSpeed : walkSpeed);
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}

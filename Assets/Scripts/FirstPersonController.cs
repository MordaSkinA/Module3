using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour, IPossessable
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float runMultiplier = 3f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private Module1 playerInput;
    private CharacterController characterController;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isRunPressed;
    private float pitch;

    private void Awake()
    {
        playerInput = new Module1();
        characterController = GetComponent<CharacterController>();

        playerInput.CharacterControls.Move.performed += OnMoveInput;
        playerInput.CharacterControls.Move.canceled += OnMoveInput;
        playerInput.CharacterControls.Look.performed += OnLookInput;
        playerInput.CharacterControls.Look.canceled += OnLookInput;
        playerInput.CharacterControls.Run.started += OnRunInput;
        playerInput.CharacterControls.Run.canceled += OnRunInput;
    }
    
    public GameObject GetCameraObject()
    {
        return cameraTransform.gameObject;
    }

    public void OnPossess()
    {
        playerInput.CharacterControls.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnUnpossess()
    {
        playerInput.CharacterControls.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        Look();
        Move();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnRunInput(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    private void Look()
    {
        float yaw = lookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up, yaw);

        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction = Vector3.ClampMagnitude(direction, 1f);

        float speed = moveSpeed * (isRunPressed ? runMultiplier : 1f);
        characterController.Move(direction * speed * Time.deltaTime);
    }
}
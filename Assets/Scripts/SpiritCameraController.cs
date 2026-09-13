using UnityEngine;
using UnityEngine.InputSystem;

public class SpiritCameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 50f;

    private Module1 spiritInput;
    private Vector2 moveInput;
    private float zoomInput;

    private void Awake()
    {
        spiritInput = new Module1();

        spiritInput.SpiritControls.Move.performed += OnMoveInput;
        spiritInput.SpiritControls.Move.canceled += OnMoveInput;
        spiritInput.SpiritControls.Zoom.performed += OnZoomInput;
        spiritInput.SpiritControls.Zoom.canceled += OnZoomInput;
    }

    private void OnEnable()
    {
        spiritInput.SpiritControls.Enable();
    }

    private void OnDisable()
    {
        spiritInput.SpiritControls.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnZoomInput(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        Vector3 horizontalMove = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.position += horizontalMove * moveSpeed * Time.deltaTime;
    }

    private void Zoom()
    {
        if (Mathf.Approximately(zoomInput, 0f))
        {
            return;
        }

        Vector3 newPosition = transform.position + transform.forward * zoomInput * zoomSpeed * Time.deltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, minHeight, maxHeight);
        transform.position = newPosition;
    }
}
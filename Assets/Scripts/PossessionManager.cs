using UnityEngine;
using UnityEngine.InputSystem;

public interface IPossessable
{
    void OnPossess();
    void OnUnpossess();
    GameObject GetCameraObject();
}

public class PossessionManager : MonoBehaviour
{
    [SerializeField] private LayerMask possessableLayer;
    [SerializeField] private Camera spiritCamera;
    [SerializeField] private GameObject spiritCameraObject;
    private GameObject currentCameraObject;

    private Module1 input;
    private MonoBehaviour currentBody;

    private void Awake()
    {
        input = new Module1();
        input.SpiritControls.Click.performed += OnClickInput;
        input.GlobalControls.Exit.performed += OnExitInput;
    }

    private void Start()
    {
        spiritCameraObject.SetActive(true);
    }

    private void OnEnable()
    {
        input.SpiritControls.Enable();
        input.GlobalControls.Enable();
    }

    private void OnDisable()
    {
        input.SpiritControls.Disable();
        input.GlobalControls.Disable();
    }

    private void OnClickInput(InputAction.CallbackContext context)
    {
        if (currentBody != null)
        {
            return;
        }

        TryPossessAtCursor();
    }

    private void OnExitInput(InputAction.CallbackContext context)
    {
        if (currentBody == null)
        {
            return;
        }

        Unpossess();
    }

    private void TryPossessAtCursor()
    {
        Ray ray = spiritCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, possessableLayer))
        {
            IPossessable possessable = hit.collider.GetComponent<IPossessable>();
            MonoBehaviour target = possessable as MonoBehaviour;

            if (possessable != null)
            {
                Possess(target, possessable);
            }
        }
    }

    private void Possess(MonoBehaviour newBody, IPossessable possessable)
    {
        newBody.enabled = true;
        possessable.OnPossess();
        currentBody = newBody;

        spiritCameraObject.SetActive(false);
        currentCameraObject = possessable.GetCameraObject();
        currentCameraObject.SetActive(true);

        input.SpiritControls.Disable();
    }

    private void Unpossess()
    {
        if (currentBody is IPossessable possessable)
        {
            possessable.OnUnpossess();
        }

        currentBody.enabled = false;
        currentBody = null;

        currentCameraObject.SetActive(false);
        currentCameraObject = null;
        spiritCameraObject.SetActive(true);

        input.SpiritControls.Enable();
    }
}
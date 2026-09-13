using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Wallet))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(FirstPersonController))]
public class PlayerTraderInteractor : MonoBehaviour
{
    private Module1 playerInput;
    private FirstPersonController firstPersonController;
    private Wallet wallet;
    private Inventory inventory;
    private Trader currentTrader;

    private void Awake()
    {
        playerInput = new Module1();
        firstPersonController = GetComponent<FirstPersonController>();
        wallet = GetComponent<Wallet>();
        inventory = GetComponent<Inventory>();

        playerInput.CharacterControls.Interact.performed += OnInteractPerformed;
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        Trader trader = other.GetComponent<Trader>();

        if (trader != null)
        {
            currentTrader = trader;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Trader trader = other.GetComponent<Trader>();

        if (trader == currentTrader)
        {
            currentTrader = null;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!firstPersonController.enabled)
        {
            return;
        }

        if (currentTrader == null)
        {
            return;
        }

        currentTrader.TryBuy(wallet, inventory);
    }
}
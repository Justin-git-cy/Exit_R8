// Assets/Scripts/Interaction/PlayerInteractor.cs
// Put this on YOUR PLAYER'S CAMERA - whatever camera your Starter Assets
// First Person controller uses (usually a child object like "PlayerCameraRoot"
// or "Main Camera" under the Starter Assets player prefab).
// This script does NOT care what moves the player - it only raycasts from
// wherever it's attached and reports interactions. Fully independent of
// whatever controller/movement script you're using.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ExitR8.Interaction
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Look Source")]
        [Tooltip("Usually just drag this same GameObject's Camera component here.")]
        [SerializeField] private Camera playerCamera;

        [Header("Raycast Settings")]
        [SerializeField] private float interactRange = 2.5f;
        [SerializeField] private LayerMask interactableMask = ~0;

        [Header("Input")]
        [Tooltip("Optional: drag the 'Interact' action from your Input Actions asset. If left empty, falls back to the E key.")]
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private KeyCode fallbackKey = KeyCode.E;

        [Header("UI Hooks (optional)")]
        public UnityEvent<string> OnPromptChanged;
        public UnityEvent<string> OnClueShown;

        private IInteractable currentTarget;

        private void OnEnable()
        {
            if (interactAction != null)
            {
                interactAction.action.Enable();
                interactAction.action.performed += OnInteractPerformed;
            }
        }

        private void OnDisable()
        {
            if (interactAction != null)
                interactAction.action.performed -= OnInteractPerformed;
        }

        private void Update()
        {
            UpdateLookTarget();

            // Fallback path if no Input Action is wired - just use a plain key.
            if (interactAction == null && Input.GetKeyDown(fallbackKey))
                currentTarget?.Interact(this);
        }

        private void UpdateLookTarget()
        {
            IInteractable found = null;

            if (playerCamera != null &&
                Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
                    out RaycastHit hit, interactRange, interactableMask))
            {
                found = hit.collider.GetComponentInParent<IInteractable>();
            }

            if (found != currentTarget)
            {
                currentTarget = found;
                OnPromptChanged?.Invoke(currentTarget != null ? currentTarget.GetPrompt() : "");
            }
        }

        private void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            currentTarget?.Interact(this);
        }

        public void ShowClue(string text) => OnClueShown?.Invoke(text);
    }
}

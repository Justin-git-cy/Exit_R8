// Assets/Scripts/Player/FirstPersonController.cs
// Basic first-person controller: WASD/stick move via CharacterController, mouse/stick look
// via a camera pivot child transform. Uses Unity's new Input System (Send Messages behavior).
//
// Designed to slot in next to WakeUpSequence.cs:
//  - WakeUpSequence disables this script at scene start, drives cameraPivot itself
//    during the lying-down/standing-up beat, then re-enables this script when done.
//  - Assign the SAME cameraPivot transform in both components' inspector fields.

using UnityEngine;
using UnityEngine.InputSystem;

namespace ExitR8.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Child transform that holds the Camera. Must match the one on WakeUpSequence.")]
        [SerializeField] private Transform cameraPivot;

        [Header("Movement")]
        public float moveSpeed = 4f;
        public float gravity = -9.81f;

        [Header("Look")]
        public float lookSensitivity = 0.12f;
        public float minPitch = -80f;
        public float maxPitch = 80f;

        [Header("Cursor")]
        [Tooltip("Lock and hide the cursor when this script becomes active.")]
        [SerializeField] private bool lockCursorOnEnable = true;

        private CharacterController controller;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float pitch;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            if (lockCursorOnEnable)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            // Pick up whatever pitch WakeUpSequence left the camera at (e.g. 0 after standing),
            // so the view doesn't snap the first time this script drives the camera.
            if (cameraPivot != null)
                pitch = NormalizePitch(cameraPivot.localEulerAngles.x);

            verticalVelocity = 0f;
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
        }

        private void Update()
        {
            HandleLook();
            HandleMove();
        }

        private void HandleLook()
        {
            if (cameraPivot == null) return;

            float yaw = lookInput.x * lookSensitivity;
            pitch -= lookInput.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.Rotate(Vector3.up * yaw);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            move *= moveSpeed;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f; // small downward bias keeps the controller grounded

            verticalVelocity += gravity * Time.deltaTime;
            move.y = verticalVelocity;

            controller.Move(move * Time.deltaTime);
        }

        private static float NormalizePitch(float rawX)
        {
            // localEulerAngles wraps to 0-360; convert back to a signed -180..180 pitch.
            return rawX > 180f ? rawX - 360f : rawX;
        }
    }
}

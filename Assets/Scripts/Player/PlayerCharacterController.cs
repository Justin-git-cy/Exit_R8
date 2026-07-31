// Assets/Scripts/Player/PlayerCharacterController.cs
// Self-contained first-person movement. Spawns the player at an explicit
// SpawnPoint transform (place this in open floor space, away from walls/bed)
// instead of trusting a hardcoded position — this avoids the CharacterController
// getting re-enabled while overlapping geometry, which is what causes players
// to clip into or get shoved through walls.
//
// No dependency on WakeUpSequence or anything else — just movement + look +
// a safe spawn. Attach this alongside your existing interaction scripts.

using UnityEngine;
using UnityEngine.InputSystem;

namespace ExitR8.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("Spawn")]
        [Tooltip("Empty GameObject placed in open floor space (room center), NOT on/near the bed or walls.")]
        [SerializeField] private Transform spawnPoint;

        [Header("References")]
        [Tooltip("Child transform holding the Camera. Used for look pitch.")]
        [SerializeField] private Transform cameraPivot;

        [Header("Movement")]
        public float moveSpeed = 4f;
        public float gravity = -9.81f;
        [Tooltip("Small downward force applied while grounded, keeps the controller planted.")]
        public float groundedStickForce = -2f;

        [Header("Look")]
        public float lookSensitivity = 0.12f;
        public float minPitch = -80f;
        public float maxPitch = 80f;

        [Header("Cursor")]
        [SerializeField] private bool lockCursor = true;

        private CharacterController controller;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private float pitch;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            SpawnAtStart();

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // Disabling the CharacterController before moving the transform, then
        // re-enabling it, is what prevents Unity from trying to resolve a
        // collision the instant it wakes up in the new spot.
        private void SpawnAtStart()
        {
            if (spawnPoint == null)
            {
                Debug.LogWarning("[PlayerCharacterController] No spawnPoint assigned — player will stay wherever it was placed in the scene.");
                return;
            }

            controller.enabled = false;
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            controller.enabled = true;

            verticalVelocity = 0f;
            pitch = 0f;
            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.identity;
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
            float yaw = lookInput.x * lookSensitivity;
            transform.Rotate(Vector3.up * yaw);

            if (cameraPivot == null) return;

            pitch -= lookInput.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            move *= moveSpeed;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = groundedStickForce;

            verticalVelocity += gravity * Time.deltaTime;
            move.y = verticalVelocity;

            // CharacterController.Move handles wall/floor collision resolution itself —
            // this is what stops the player walking through walls, as long as the
            // collider isn't already overlapping geometry when it starts.
            controller.Move(move * Time.deltaTime);
        }

        // Call this from a respawn/reset flow (e.g. after a loop reset) instead
        // of manually setting transform.position elsewhere in the codebase.
        public void ResetToSpawn()
        {
            SpawnAtStart();
        }
    }
}

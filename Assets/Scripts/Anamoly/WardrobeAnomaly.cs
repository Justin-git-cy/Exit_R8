// Assets/Scripts/Anamoly/WardrobeAnomaly.cs
using UnityEngine;
using System.Collections;
using ExitR8.Interaction;
using ExitR8.Loop;

namespace ExitR8.Anamoly
{
    [RequireComponent(typeof(Collider))]
    public class WardrobeAnomaly : MonoBehaviour, IInteractable
    {
        [Header("Wardrobe Slide Settings")]
        [Tooltip("How far to the left the wardrobe slides (negative X direction).")]
        [SerializeField] private float slideDistance = -1.2f;
        [SerializeField] private float slideSpeed = 2f;

        [Header("Prompt")]
        [SerializeField] private string closedPrompt = "Examine Wardrobe";
        [SerializeField] private string openPrompt = "(Wardrobe — examined)";

        private bool isOpen = false;
        private bool hasBeenInspected = false;
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private Coroutine slideCoroutine;

        private void Awake()
        {
            startPosition = transform.position;
            // Slide along the local X-axis (negative for leftward slide)
            targetPosition = startPosition + transform.right * slideDistance;
        }

        public void ResetPosition()
        {
            isOpen = false;
            hasBeenInspected = false;
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);
            transform.position = startPosition;
        }

        public string GetPrompt()
        {
            return hasBeenInspected ? openPrompt : closedPrompt;
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (LoopManager.Instance == null) return;

            int currentStage = LoopManager.Instance.CurrentStageIndex;
            bool isAnomaly = LoopManager.Instance.IsStageAnomaly(currentStage);
            int anomalyType = LoopManager.Instance.GetAnomalyTypeForStage(currentStage);

            string observation;

            // Slide open on interact
            if (!isOpen)
            {
                isOpen = true;
                if (slideCoroutine != null) StopCoroutine(slideCoroutine);
                slideCoroutine = StartCoroutine(SlideTo(targetPosition));
            }

            // Check if this is the active wardrobe anomaly stage
            if (isAnomaly && anomalyType == 5)
            {
                int correctDoor = LoopManager.Instance.GetCorrectDoorForStage(currentStage);
                int displayDoor = correctDoor + 1;

                observation = $"Anomaly Detected — The wardrobe slides aside to reveal a hidden door code: '{displayDoor}'.";
                hasBeenInspected = true;
            }
            else
            {
                observation = "The wardrobe slides aside, but nothing is behind it. It looks normal.";
                hasBeenInspected = true;
            }

            if (interactor != null) interactor.ShowClue(observation);
            LoopManager.Instance.MarkClueDiscovered(observation);

            Debug.Log($"[WardrobeAnomaly] {observation}");
        }

        private IEnumerator SlideTo(Vector3 target)
        {
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * slideSpeed);
                yield return null;
            }
            transform.position = target;
        }
    }
}

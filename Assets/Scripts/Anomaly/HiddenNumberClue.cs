// Assets/Scripts/Anamoly/HiddenNumberClue.cs
// Attach to objects that reveal a hidden door number (Bed, Ceiling Text).
// Only reveals the number when the matching anomaly type is active for this stage.
using UnityEngine;
using TMPro;
using ExitR8.Interaction;
using ExitR8.Loop;

namespace ExitR8.Anamoly
{
    [RequireComponent(typeof(Collider))]
    public class HiddenNumberClue : MonoBehaviour, IInteractable
    {
        [Header("Object Description")]
        [SerializeField] private string objectName = "Object";
        [SerializeField] private string inspectPrompt = "Examine";

        [Header("Anomaly Type")]
        [Tooltip("Which anomaly type this clue belongs to. 2=Bed, 3=Ceiling")]
        [Range(0, 7)]
        [SerializeField] private int anomalyTypeId = 2;

        [Header("Clue Visibility")]
        [Tooltip("The hidden TextMeshPro or visual that appears when the anomaly is active.")]
        [SerializeField] private GameObject hiddenVisual;

        private bool hasBeenInspected = false;

        private void Start()
        {
            if (hiddenVisual != null)
                hiddenVisual.SetActive(false);
        }

        public string GetPrompt()
        {
            return hasBeenInspected ? $"({objectName} — examined)" : inspectPrompt;
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (LoopManager.Instance == null) return;

            int currentStage = LoopManager.Instance.CurrentStageIndex;
            bool isAnomaly = LoopManager.Instance.IsStageAnomaly(currentStage);
            int stageAnomalyType = LoopManager.Instance.GetAnomalyTypeForStage(currentStage);
            int correctDoor = LoopManager.Instance.GetCorrectDoorForStage(currentStage) + 1;

            string observation;

            // Only reveal hidden number if this stage's anomaly matches this object
            if (isAnomaly && stageAnomalyType == anomalyTypeId)
            {
                if (hiddenVisual != null)
                    hiddenVisual.SetActive(true);

                if (!hasBeenInspected)
                {
                    observation = $"Anomaly Detected — Found the number '{correctDoor}' on the {objectName}.";
                    hasBeenInspected = true;
                }
                else
                {
                    observation = $"The {objectName} shows the door code '{correctDoor}'.";
                }
            }
            else
            {
                // Not the right anomaly or normal room
                observation = $"The {objectName} looks normal. Nothing unusual.";
                hasBeenInspected = true;
            }

            if (interactor != null) interactor.ShowClue(observation);
            LoopManager.Instance.MarkClueDiscovered(observation);

            Debug.Log($"[HiddenNumberClue:{objectName}] {observation}");
        }
    }
}

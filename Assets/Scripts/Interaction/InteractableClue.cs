// Assets/Scripts/Interaction/InteractableClue.cs
// Universal room observation point. Attach to ANY inspectable object (painting, carpet, mirror, window, bed, etc.).
// When the player presses E on this object, it generates context-aware observation text
// based on whether this stage has an anomaly and what type it is.
// Examining ANY object in the room unlocks the doors for that stage.
using UnityEngine;
using ExitR8.Loop;

namespace ExitR8.Interaction
{
    [RequireComponent(typeof(Collider))]
    public class InteractableClue : MonoBehaviour, IInteractable
    {
        [Header("Object Identity")]
        [Tooltip("Display name for this object (e.g. Painting, Carpet, Mirror, Window).")]
        public string objectName = "Room Observation Point";

        [Header("Anomaly Type Link")]
        [Tooltip("Which anomaly type this object represents. -1 = generic/unlinked. " +
                 "0=Audio, 1=Painting, 2=Bed, 3=Ceiling, 4=Carpet, 5=Wardrobe, 6=Mirror/Window, 7=CeilingLight")]
        [Range(-1, 7)]
        public int anomalyTypeId = -1;

        [Header("Custom Text (Optional)")]
        [TextArea(2, 4)]
        public string customObservationText;

        private bool hasBeenInspected = false;

        public string GetPrompt()
        {
            if (hasBeenInspected)
                return $"({objectName} — examined)";
            return $"Examine {objectName}";
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (LoopManager.Instance == null) return;

            int currentStage = LoopManager.Instance.CurrentStageIndex;
            bool isAnomaly = LoopManager.Instance.IsStageAnomaly(currentStage);
            int stageAnomalyType = LoopManager.Instance.GetAnomalyTypeForStage(currentStage);
            int correctDoor = LoopManager.Instance.GetCorrectDoorForStage(currentStage);
            int displayDoor = correctDoor + 1;

            string observation;

            // If custom text is set, always use that
            if (!string.IsNullOrEmpty(customObservationText))
            {
                observation = customObservationText;
            }
            // If this IS an anomaly stage and THIS object matches the active anomaly type
            else if (isAnomaly && anomalyTypeId == stageAnomalyType)
            {
                observation = GetAnomalyObservation(stageAnomalyType, displayDoor);
            }
            // If this IS an anomaly stage but this object is NOT the anomaly source
            else if (isAnomaly && anomalyTypeId != stageAnomalyType)
            {
                observation = $"The {objectName} looks normal. Nothing unusual here.";
            }
            // Normal room — no anomaly at all
            else
            {
                observation = $"The {objectName} appears completely normal. No anomaly detected.";
            }

            hasBeenInspected = true;

            if (interactor != null) interactor.ShowClue(observation);
            LoopManager.Instance.MarkClueDiscovered(observation);

            Debug.Log($"[InteractableClue:{objectName}] {observation}");
        }

        private string GetAnomalyObservation(int anomalyType, int displayDoor)
        {
            switch (anomalyType)
            {
                case 0:
                    return "Anomaly Detected — Strange audio pitch distortion near the doors.";
                case 1:
                    return "Anomaly Detected — The painting is tilted at an unnatural angle.";
                case 2:
                    return $"Anomaly Detected — A number '{displayDoor}' is scratched onto the bed frame.";
                case 3:
                    return $"Anomaly Detected — The number '{displayDoor}' is written on the ceiling.";
                case 4:
                    return "Anomaly Detected — The carpet color has shifted. It's not the usual grey.";
                case 6:
                    return "Anomaly Detected — The mirror shows an arrow, and the window has an odd tint.";
                case 7:
                    return "Anomaly Detected — The ceiling light is glowing an unusual color.";
                default:
                    return "Anomaly Detected — Something is off in this room.";
            }
        }
    }
}

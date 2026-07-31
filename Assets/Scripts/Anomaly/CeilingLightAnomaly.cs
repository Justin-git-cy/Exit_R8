// Assets/Scripts/Anamoly/CeilingLightAnomaly.cs
using UnityEngine;
using ExitR8.Interaction;
using ExitR8.Loop;

namespace ExitR8.Anamoly
{
    [RequireComponent(typeof(Collider))]
    public class CeilingLightAnomaly : MonoBehaviour, IInteractable
    {
        [Header("Light Reference")]
        [Tooltip("The ceiling Light component that changes color during this anomaly.")]
        [SerializeField] private Light ceilingLight;

        [Header("Prompt")]
        [SerializeField] private string inspectPrompt = "Examine Ceiling Light";
        [SerializeField] private string inspectedPrompt = "(Ceiling Light — examined)";

        private bool hasBeenInspected = false;

        public string GetPrompt()
        {
            return hasBeenInspected ? inspectedPrompt : inspectPrompt;
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (LoopManager.Instance == null) return;

            int currentStage = LoopManager.Instance.CurrentStageIndex;
            bool isAnomaly = LoopManager.Instance.IsStageAnomaly(currentStage);
            int anomalyType = LoopManager.Instance.GetAnomalyTypeForStage(currentStage);

            string observation;

            if (isAnomaly && anomalyType == 7)
            {
                int correctDoor = LoopManager.Instance.GetCorrectDoorForStage(currentStage);
                string colorName = (correctDoor == 0) ? "red" : "blue";
                int displayDoor = correctDoor + 1;

                if (!hasBeenInspected)
                {
                    observation = $"The ceiling light is glowing {colorName}... that's not normal. Door {displayDoor} feels safe.";
                    hasBeenInspected = true;
                }
                else
                {
                    observation = $"The ceiling light is still {colorName}. Door {displayDoor} is the way.";
                }
            }
            else
            {
                observation = "The ceiling light looks normal. Nothing unusual.";
                hasBeenInspected = true;
            }

            if (interactor != null) interactor.ShowClue(observation);
            LoopManager.Instance.MarkClueDiscovered(observation);

            Debug.Log($"[CeilingLightAnomaly] {observation}");
        }
    }
}

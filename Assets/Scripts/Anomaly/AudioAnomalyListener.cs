// Assets/Scripts/Anamoly/AudioAnomalyListener.cs
using UnityEngine;
using ExitR8.Interaction;
using ExitR8.Loop;

namespace ExitR8.Anamoly
{
    [RequireComponent(typeof(Collider))]
    public class AudioAnomalyListener : MonoBehaviour, IInteractable
    {
        [Header("Door Identity")]
        [Range(0, 2)] public int doorIndex;

        [Header("Audio Reference")]
        [SerializeField] private AudioSource doorAudioSource;

        [Header("Prompt Text")]
        [SerializeField] private string listenPrompt = "Listen at door";

        public string GetPrompt() => listenPrompt;

        public void Interact(PlayerInteractor interactor)
        {
            if (LoopManager.Instance == null) return;

            int currentStage = LoopManager.Instance.CurrentStageIndex;
            int correctDoor = LoopManager.Instance.GetCorrectDoorForStage(currentStage);

            bool isCorrectDoor = (correctDoor == doorIndex);
            string observation;

            if (isCorrectDoor)
            {
                observation = $"Door {doorIndex + 1}: The sound behind this door matches the room perfectly. It feels right.";
            }
            else
            {
                if (doorAudioSource != null && Mathf.Abs(doorAudioSource.pitch - 1.0f) > 0.05f)
                {
                    observation = $"Door {doorIndex + 1}: Pitch is distorted like a warped tape. This door is wrong.";
                }
                else
                {
                    observation = $"Door {doorIndex + 1}: Strange audio anomaly behind this door.";
                }
            }

            interactor.ShowClue(observation);
            LoopManager.Instance.MarkClueDiscovered(observation);

            Debug.Log($"[AudioAnomalyListener] {observation}");
        }
    }
}

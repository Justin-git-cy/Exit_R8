// Assets/Scripts/Room/Door.cs
using UnityEngine;
using ExitR8.Interaction;
using ExitR8.Loop;

namespace ExitR8.Room
{
    [RequireComponent(typeof(Collider))]
    public class Door : MonoBehaviour, IInteractable
    {
        [Tooltip("Door index: 0 for Door_0, 1 for Door_1, 2 for Showcase Door_2")]
        [Range(0, 2)] public int doorIndex;

        public string GetPrompt()
        {
            if (doorIndex == 2)
            {
                return "Passed Through (Showcase Only)";
            }

            if (LoopManager.Instance != null && !LoopManager.Instance.CurrentClueDiscovered)
            {
                return "Door Locked (Observe room / find clue first!)";
            }
            return $"Open Door {doorIndex + 1}";
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (doorIndex == 2)
            {
                Debug.Log("[Door] Door 3 is a showcase door and cannot be interacted with.");
                return;
            }

            if (LoopManager.Instance == null) return;

            // Enforce requirement: Player MUST observe the clue first
            if (!LoopManager.Instance.CurrentClueDiscovered)
            {
                string warningMsg = "The door is locked... Inspect the room to verify if an anomaly is present!";
                if (interactor != null) interactor.ShowClue(warningMsg);
                Debug.LogWarning("[Door] Blocked: Player tried to open door without observing room first.");
                return;
            }

            // Player has found the clue, test if this door is correct or wrong
            LoopManager.Instance.AttemptDoorOpen(doorIndex);
        }
    }
}

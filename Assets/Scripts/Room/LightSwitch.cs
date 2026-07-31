// Assets/Scripts/Room/LightSwitch.cs
using UnityEngine;
using ExitR8.Interaction;

namespace ExitR8.Room
{
    [RequireComponent(typeof(Collider))]
    public class LightSwitch : MonoBehaviour, IInteractable
    {
        [SerializeField] private Light targetLight;
        [SerializeField] private bool startsOn = true;
        private bool isOn;

        private void Start() { isOn = startsOn; ApplyState(); }
        public string GetPrompt() => isOn ? "Turn off light" : "Turn on light";
        public void Interact(PlayerInteractor interactor) { isOn = !isOn; ApplyState(); }
        private void ApplyState() { if (targetLight != null) targetLight.enabled = isOn; }
    }
}

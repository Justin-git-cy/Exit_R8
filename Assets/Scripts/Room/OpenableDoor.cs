// Assets/Scripts/Room/OpenableDoor.cs
// Put on a hinge pivot (wardrobe door or box lid). Child = the door/lid mesh.
using UnityEngine;
using System.Collections;
using ExitR8.Interaction;

namespace ExitR8.Room
{
    public class OpenableDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private string closedPrompt = "Open";
        [SerializeField] private string openPrompt = "Close";
        [SerializeField] private float openAngle = -90f;
        [SerializeField] private float openSpeed = 3f;

        private bool isOpen;
        private Quaternion closedRotation;
        private Quaternion openRotation;
        private Coroutine swingRoutine;

        private void Start()
        {
            closedRotation = transform.localRotation;
            openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        }

        public string GetPrompt() => isOpen ? openPrompt : closedPrompt;

        public void Interact(PlayerInteractor interactor)
        {
            isOpen = !isOpen;
            if (swingRoutine != null) StopCoroutine(swingRoutine);
            swingRoutine = StartCoroutine(SwingTo(isOpen ? openRotation : closedRotation));
        }

        private IEnumerator SwingTo(Quaternion target)
        {
            while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * openSpeed);
                yield return null;
            }
            transform.localRotation = target;
        }
    }
}

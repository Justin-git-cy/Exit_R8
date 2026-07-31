// Assets/Scripts/UI/DeathScreenUI.cs
// Put on the same object as LoopManager (or anywhere in the Room scene).
using UnityEngine;
using ExitR8.Loop;

namespace ExitR8.UI
{
    public class DeathScreenUI : MonoBehaviour
    {
        [TextArea(1, 2)]
        [SerializeField] private string message = "That wasn't the way out... back to the start.";
        [SerializeField] private float displayDuration = 1.6f;

        private bool showing;
        private float timer;

        private void OnEnable() { if (LoopManager.Instance != null) LoopManager.Instance.OnPlayerDied += HandleDied; }
        private void OnDisable() { if (LoopManager.Instance != null) LoopManager.Instance.OnPlayerDied -= HandleDied; }
        private void HandleDied() { showing = true; timer = displayDuration; }

        private void Update()
        {
            if (!showing) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) showing = false;
        }

        private void OnGUI()
        {
            if (!showing) return;
            GUI.color = new Color(0.3f, 0f, 0f, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            var style = new GUIStyle(GUI.skin.label) { fontSize = 32, alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(0, Screen.height / 2f - 40, Screen.width, 80), message, style);
        }
    }
}

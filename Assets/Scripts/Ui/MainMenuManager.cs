// Assets/Scripts/UI/MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExitR8.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Scene Configuration")]
        [SerializeField] private string roomSceneName = "RoomUlt";

        [Header("Rules UI Overlay (Optional)")]
        [Tooltip("If assigned, clicking Play will show this Rules Panel first before loading the scene.")]
        [SerializeField] private GameObject rulesPanelOverlay;

        public void PlayGame()
        {
            if (rulesPanelOverlay != null)
            {
                // Show rules overlay first
                rulesPanelOverlay.SetActive(true);
            }
            else
            {
                // Load room scene directly
                ConfirmAndStartGame();
            }
        }

        public void ConfirmAndStartGame()
        {
            if (rulesPanelOverlay != null)
            {
                rulesPanelOverlay.SetActive(false);
            }
            SceneManager.LoadScene(roomSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

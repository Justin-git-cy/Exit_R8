// Assets/Scripts/UI/EndingSceneController.cs
// Put on an object in your NextArea (win) scene.
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitR8.Loop;

namespace ExitR8.UI
{
    public class EndingSceneController : MonoBehaviour
    {
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private void Start()
        {
            if (LoopManager.Instance != null) LoopManager.Instance.ResetMemory();
            Time.timeScale = 1f;
        }

        private void Update()
        {
            // Force cursor unlocked every frame so the FPS controller can't override it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Press Enter or Return to go back to Main Menu
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ReturnToMainMenu();
            }
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            if (LoopManager.Instance != null) LoopManager.Instance.ResetMemory();
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}

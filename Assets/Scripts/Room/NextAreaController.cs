using UnityEngine;
using UnityEngine.SceneManagement;

public class NextAreaController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // Hook this up to the Main Menu button's OnClick() in the Inspector
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

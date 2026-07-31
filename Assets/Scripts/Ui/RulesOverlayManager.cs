// Assets/Scripts/UI/RulesOverlayManager.cs
using UnityEngine;

public class RulesOverlayManager : MonoBehaviour
{
    [Header("UI Panel Reference")]
    [Tooltip("Drag your Rule_Panel GameObject here.")]
    public GameObject rulesPanel;

    [Header("Keyboard Shortcuts")]
    public KeyCode closeKey = KeyCode.Return;
    public KeyCode spaceKey = KeyCode.Space;
    public KeyCode interactKey = KeyCode.E;

    private static bool hasShownRules = false;

    private void Awake()
    {
        if (rulesPanel == null)
        {
            rulesPanel = gameObject;
        }
    }

    private void Start()
    {
        // Only show rules on Stage 1 (StageIndex == 0)
        if (ExitR8.Loop.LoopManager.Instance != null && ExitR8.Loop.LoopManager.Instance.CurrentStageIndex == 0)
        {
            ShowRules();
        }
        else
        {
            StartGameplay(); // Auto-start for Stage 2+
        }
    }

    private void Update()
    {
        if (rulesPanel != null && rulesPanel.activeSelf)
        {
            if (Input.GetKeyDown(closeKey) || Input.GetKeyDown(spaceKey) || Input.GetKeyDown(interactKey))
            {
                StartGameplay();
            }
        }
    }

    public void ShowRules()
    {
        if (rulesPanel != null) rulesPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGameplay()
    {
        hasShownRules = true;
        if (rulesPanel != null) rulesPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void ResetRulesState()
    {
        hasShownRules = false;
    }
}

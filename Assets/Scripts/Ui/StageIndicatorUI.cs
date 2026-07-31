// Assets/Scripts/UI/StageIndicatorUI.cs
using UnityEngine;
using TMPro;
using System.Collections;
using ExitR8.Loop;

public class StageIndicatorUI : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("TextMeshProUGUI component for displaying Stage text (e.g. STAGE 1).")]
    [SerializeField] private TextMeshProUGUI stageText;

    [Header("Display Timings")]
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeDuration = 1.0f;

    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.OnStageStarted += ShowStagePopup;
        }
    }

    private void OnDisable()
    {
        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.OnStageStarted -= ShowStagePopup;
        }
    }

    private void Awake()
    {
        if (stageText == null)
        {
            stageText = GetComponent<TextMeshProUGUI>();
            if (stageText == null)
            {
                stageText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    private void Start()
    {
        int currentStage = 0;
        if (LoopManager.Instance != null)
        {
            currentStage = LoopManager.Instance.CurrentStageIndex;
        }
        ShowStagePopup(currentStage);
    }

    public void ShowStagePopup(int stageIndex)
    {
        if (stageText == null) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        stageText.text = $"STAGE {stageIndex + 1}";
        stageText.gameObject.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        Color originalColor = stageText.color;
        originalColor.a = 1f;
        stageText.color = originalColor;

        // Wait visible duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out alpha
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            stageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        stageText.gameObject.SetActive(false);
        stageText.color = originalColor; // Restore alpha for next trigger
    }
}

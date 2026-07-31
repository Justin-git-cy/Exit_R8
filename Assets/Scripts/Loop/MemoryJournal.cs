// Assets/Scripts/Loop/MemoryJournal.cs
// Upgraded: Full journal UI using TextMeshPro, stage-grouped entries, reset on death.
using UnityEngine;
using TMPro;
using ExitR8.Loop;

namespace ExitR8.UI
{
    public class MemoryJournalUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Panel GameObject that contains the journal display. Will be toggled on/off.")]
        [SerializeField] private GameObject journalPanel;
        [Tooltip("TextMeshProUGUI component inside the journal panel for displaying entries.")]
        [SerializeField] private TextMeshProUGUI journalContentText;

        [Header("Controls")]
        [SerializeField] private KeyCode toggleKey = KeyCode.J;

        private bool isOpen;

        private void Start()
        {
            if (journalPanel != null)
                journalPanel.SetActive(false);

            if (LoopManager.Instance != null)
                LoopManager.Instance.OnJournalUpdated += RefreshDisplay;
        }

        private void OnDestroy()
        {
            if (LoopManager.Instance != null)
                LoopManager.Instance.OnJournalUpdated -= RefreshDisplay;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                isOpen = !isOpen;
                if (journalPanel != null)
                    journalPanel.SetActive(isOpen);

                if (isOpen) RefreshDisplay();
            }
        }

        private void RefreshDisplay()
        {
            if (journalContentText == null) return;
            if (LoopManager.Instance == null)
            {
                journalContentText.text = "<i>No data available.</i>";
                return;
            }

            var entries = LoopManager.Instance.JournalEntries;
            int currentStage = LoopManager.Instance.CurrentStageIndex + 1;
            int totalStages = LoopManager.Instance.TotalStages;

            string body = $"<b><size=28>=== MEMORY JOURNAL ===</size></b>\n";
            body += $"<size=20>Stage {currentStage} / {totalStages}</size>\n\n";

            if (entries.Count == 0)
            {
                body += "<i>Journal is empty.\nExplore the room to discover clues.</i>";
            }
            else
            {
                foreach (var entry in entries)
                {
                    body += $"<color=#FFD700>[Stage {entry.stageNumber} — {entry.timestamp}]</color>\n";
                    body += $"  {entry.observationText}\n\n";
                }
            }

            journalContentText.text = body;
        }

        // Memory Journal UI component handles TextMeshProUGUI panel
    }
}

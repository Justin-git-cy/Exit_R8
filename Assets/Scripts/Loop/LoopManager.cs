// Assets/Scripts/Loop/LoopManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace ExitR8.Loop
{
    public class LoopManager : MonoBehaviour
    {
        public static LoopManager Instance { get; private set; }

        [Header("Scene Configuration")]
        [SerializeField] private string roomSceneName = "RoomUlt";
        [SerializeField] private string deathSceneName = "DeathScene";
        [SerializeField] private string winSceneName = "NextArea";

        [Header("Stage Settings")]
        public int maxStages = 8; // Clears game after 8 stages
        public int CurrentStageIndex { get; private set; } = 0;
        public int TotalStages => maxStages;

        // Dynamic array storing correct door index (0 or 1) for each stage
        private int[] correctDoorsPerStage;
        // Stores whether each stage has an anomaly (true) or is a Normal Room (false)
        private bool[] isAnomalyStage;
        // Stores which anomaly index (0 to 6) is active for anomaly stages
        private int[] anomalyTypePerStage;

        // Has the player examined the current room's anomaly clue?
        public bool CurrentClueDiscovered { get; private set; } = false;

        // ----- Memory Journal Data -----
        [System.Serializable]
        public class JournalEntry
        {
            public int stageNumber;
            public string observationText;
            public string timestamp;
        }

        private readonly List<JournalEntry> journalEntries = new List<JournalEntry>();
        public IReadOnlyList<JournalEntry> JournalEntries => journalEntries;

        private readonly List<string> discoveredClues = new List<string>();
        public IReadOnlyList<string> DiscoveredClues => discoveredClues;

        // Events
        public event System.Action OnPlayerDied;
        public event System.Action OnGameWon;
        public event System.Action OnJournalUpdated;
        public event System.Action<int> OnStageStarted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeStageData();
        }

        private void Start()
        {
            StartStage(CurrentStageIndex);
        }

        /// <summary>Initializes 8-stage playthrough from a pool of 14 possibilities (7 anomalies, 7 normal rooms).</summary>
        public void InitializeStageData()
        {
            correctDoorsPerStage = new int[maxStages];
            isAnomalyStage = new bool[maxStages];
            anomalyTypePerStage = new int[maxStages];

            // Pool of 14 possibilities: 7 Anomaly, 7 Normal
            List<bool> poolTypes = new List<bool>();
            for (int i = 0; i < 7; i++) poolTypes.Add(true);  // Anomaly (7 types)
            for (int i = 0; i < 7; i++) poolTypes.Add(false); // Normal Room

            // Shuffle pool
            for (int i = 0; i < poolTypes.Count; i++)
            {
                int rnd = Random.Range(i, poolTypes.Count);
                bool temp = poolTypes[i];
                poolTypes[i] = poolTypes[rnd];
                poolTypes[rnd] = temp;
            }

            // Available anomaly types (0=Audio, 1=Painting, 2=Bed, 3=Ceiling, 4=Carpet, 6=Mirror/Window, 7=CeilingLight) — No Wardrobe (5)
            List<int> availableAnomalies = new List<int> { 0, 1, 2, 3, 4, 6, 7 };
            for (int i = 0; i < availableAnomalies.Count; i++)
            {
                int rnd = Random.Range(i, availableAnomalies.Count);
                int temp = availableAnomalies[i];
                availableAnomalies[i] = availableAnomalies[rnd];
                availableAnomalies[rnd] = temp;
            }

            int anomalyPtr = 0;
            for (int i = 0; i < maxStages; i++)
            {
                isAnomalyStage[i] = poolTypes[i];

                if (isAnomalyStage[i])
                {
                    // Anomaly found -> Choose Door 0 (index 0, displayed as Door 1)
                    correctDoorsPerStage[i] = 0;
                    anomalyTypePerStage[i] = availableAnomalies[anomalyPtr % availableAnomalies.Count];
                    anomalyPtr++;
                }
                else
                {
                    // Normal Room -> Choose Door 1 (index 1, displayed as Door 2)
                    correctDoorsPerStage[i] = 1;
                    anomalyTypePerStage[i] = -1; // Normal Room
                }
            }

            Debug.Log($"[LoopManager] Playthrough initialized: 8 stages selected from 14-possibility pool (7 anomalies + 7 normal). Safe door logic: Anomaly = Door 0, Normal = Door 1.");
        }

        public int GetCorrectDoorForStage(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < correctDoorsPerStage.Length)
                return correctDoorsPerStage[stageIndex];
            return 0;
        }

        public bool IsStageAnomaly(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < isAnomalyStage.Length)
                return isAnomalyStage[stageIndex];
            return false;
        }

        public int GetAnomalyTypeForStage(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < anomalyTypePerStage.Length)
                return anomalyTypePerStage[stageIndex];
            return -1;
        }

        public void StartStage(int stageIndex)
        {
            CurrentStageIndex = stageIndex;
            CurrentClueDiscovered = false;

            OnStageStarted?.Invoke(CurrentStageIndex);
        }

        public void MarkClueDiscovered(string clueText)
        {
            CurrentClueDiscovered = true;
            RecordClue(clueText);
            LogJournalEntry(CurrentStageIndex, clueText);
            Debug.Log($"[LoopManager] Stage {CurrentStageIndex + 1} Clue Discovered: {clueText}");
        }

        public void RecordClue(string clueText)
        {
            if (string.IsNullOrEmpty(clueText)) return;
            if (!discoveredClues.Contains(clueText))
            {
                discoveredClues.Add(clueText);
            }
        }

        public void LogJournalEntry(int stage, string text)
        {
            journalEntries.Add(new JournalEntry
            {
                stageNumber = stage + 1,
                observationText = text,
                timestamp = System.DateTime.Now.ToString("HH:mm:ss")
            });
            OnJournalUpdated?.Invoke();
        }

        public void AttemptDoorOpen(int doorIndex)
        {
            // Door 2 is showcase only
            if (doorIndex == 2)
            {
                Debug.Log("[LoopManager] Door 3 is a showcase door and cannot be opened.");
                return;
            }

            if (!CurrentClueDiscovered)
            {
                Debug.LogWarning("[LoopManager] Player attempted to open door without examining the room!");
                return;
            }

            int targetCorrectDoor = GetCorrectDoorForStage(CurrentStageIndex);
            if (doorIndex == targetCorrectDoor)
            {
                PlayerWonStage();
            }
            else
            {
                PlayerDied();
            }
        }

        private void PlayerWonStage()
        {
            CurrentStageIndex++;
            if (CurrentStageIndex >= maxStages)
            {
                OnGameWon?.Invoke();
                Debug.Log("[LoopManager] All stages cleared! Loading victory scene.");
                StartCoroutine(DelayedReload(winSceneName, 1.2f));
            }
            else
            {
                StartStage(CurrentStageIndex);
                SceneManager.LoadScene(roomSceneName);
            }
        }

        private void PlayerDied()
        {
            Debug.Log("[LoopManager] Wrong door chosen! Resetting to Stage 1, shuffling sequence, wiping journal.");
            OnPlayerDied?.Invoke();
            ResetMemory();
            InitializeStageData();
            CurrentStageIndex = 0;

            if (!string.IsNullOrEmpty(deathSceneName) && Application.CanStreamedLevelBeLoaded(deathSceneName))
            {
                StartCoroutine(DelayedReload(deathSceneName, 1.5f));
            }
            else
            {
                StartStage(0);
                SceneManager.LoadScene(roomSceneName);
            }
        }

        private IEnumerator DelayedReload(string sceneName, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            SceneManager.LoadScene(sceneName);
        }

        public void ResetMemory()
        {
            discoveredClues.Clear();
            journalEntries.Clear();
            OnJournalUpdated?.Invoke();
        }
    }
}

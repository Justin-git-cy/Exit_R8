using UnityEngine;
using TMPro;
using ExitR8.Loop;

namespace ExitR8.Room
{
    public class RoomAnomalyController : MonoBehaviour
    {
        [Header("Stage: Door Audio Sources")]
        public AudioSource[] doorAudioSources = new AudioSource[3];
        public AudioClip normalAmbienceClip;
        public AudioClip anomalyAudioClip;

        [Header("Painting (Tilted Head/Rotation)")]
        public Transform paintingTransform;
        private Quaternion paintingBaseRotation;

        [Header("Bed Frame Number")]
        public TextMeshPro bedNumberText;

        [Header("Ceiling Number")]
        public TextMeshPro ceilingNumberText;

        [Header("Carpet Color")]
        public Renderer carpetRenderer;
        public Color[] doorColors = new Color[2] { Color.red, Color.blue };


        [Header("Mirror Arrow Indicator")]
        public GameObject mirrorArrowObject;
        public Transform mirrorArrowPivot;

        [Header("Window Color Code")]
        public Renderer windowRenderer;

        [Header("Ceiling Light Anomaly (Type 7)")]
        public Light ceilingLight;
        public Color lightDoor0Color = new Color(1f, 0.2f, 0.2f, 1f);  // Red tint
        public Color lightDoor1Color = new Color(0.2f, 0.3f, 1f, 1f);  // Blue tint
        private Color defaultLightColor = Color.white;

        private void Start()
        {
            if (paintingTransform != null) paintingBaseRotation = paintingTransform.localRotation;
            if (ceilingLight != null) defaultLightColor = ceilingLight.color;

            if (LoopManager.Instance != null)
            {
                ApplyStageAnomaly(LoopManager.Instance.CurrentStageIndex);
            }
        }

        public void ApplyStageAnomaly(int stageIndex)
        {
            ResetAllAnomalies();

            if (LoopManager.Instance == null) return;

            bool isAnomaly = LoopManager.Instance.IsStageAnomaly(stageIndex);
            int correctDoor = LoopManager.Instance.GetCorrectDoorForStage(stageIndex); // 0 or 1
            int displayDoorNum = correctDoor + 1; // 1 or 2 for display

            if (!isAnomaly)
            {
                Debug.Log($"[RoomAnomalyController] Stage {stageIndex + 1}: NORMAL ROOM (No Anomaly). Safe Door: Door {displayDoorNum}");
                // In a normal room, everything stays default/neutral.
                return;
            }

            int anomalyType = LoopManager.Instance.GetAnomalyTypeForStage(stageIndex);
            Debug.Log($"[RoomAnomalyController] Stage {stageIndex + 1}: ANOMALY ROOM (Type {anomalyType}). Safe Door: Door {displayDoorNum}");

            switch (anomalyType)
            {
                case 0: // Audio Anomaly
                    SetupAudioAnomaly(correctDoor);
                    break;

                case 1: // Painting Tilt (Door 0 tilt -35deg, Door 1 tilt +35deg)
                    if (paintingTransform != null)
                    {
                        float zAngle = (correctDoor == 0) ? -35f : 35f;
                        paintingTransform.localRotation = paintingBaseRotation * Quaternion.Euler(0, 0, zAngle);
                    }
                    break;

                case 2: // Bed Number
                    if (bedNumberText != null)
                    {
                        bedNumberText.gameObject.SetActive(true);
                        bedNumberText.text = displayDoorNum.ToString();
                    }
                    break;

                case 3: // Ceiling Number
                    if (ceilingNumberText != null)
                    {
                        ceilingNumberText.gameObject.SetActive(true);
                        ceilingNumberText.text = displayDoorNum.ToString();
                    }
                    break;

                case 4: // Carpet Color Code (Door 0 = Red, Door 1 = Blue)
                    if (carpetRenderer != null && correctDoor < doorColors.Length)
                    {
                        carpetRenderer.material.color = doorColors[correctDoor];
                    }
                    break;



                case 6: // Mirror Arrow / Window Tint
                    if (mirrorArrowObject != null)
                    {
                        mirrorArrowObject.SetActive(true);
                        if (mirrorArrowPivot != null)
                        {
                            float zAngle = (correctDoor == 0) ? -45f : 45f;
                            mirrorArrowPivot.localRotation = Quaternion.Euler(0, 0, zAngle);
                        }
                    }
                    if (windowRenderer != null && correctDoor < doorColors.Length)
                    {
                        windowRenderer.material.color = doorColors[correctDoor];
                    }
                    break;

                case 7: // Ceiling Light Color Anomaly
                    if (ceilingLight != null)
                    {
                        ceilingLight.color = (correctDoor == 0) ? lightDoor0Color : lightDoor1Color;
                        Debug.Log($"[RoomAnomalyController] Ceiling Light set to {ceilingLight.color} (Door {correctDoor} safe)");
                    }
                    break;
            }
        }

        private void SetupAudioAnomaly(int correctDoor)
        {
            for (int i = 0; i < 2; i++) // Doors 0 and 1
            {
                if (doorAudioSources.Length <= i || doorAudioSources[i] == null) continue;

                if (i == correctDoor)
                {
                    doorAudioSources[i].clip = normalAmbienceClip;
                    doorAudioSources[i].pitch = 1.0f;
                }
                else
                {
                    doorAudioSources[i].clip = anomalyAudioClip != null ? anomalyAudioClip : normalAmbienceClip;
                    doorAudioSources[i].pitch = 0.8f;
                }

                if (doorAudioSources[i].clip != null)
                {
                    doorAudioSources[i].loop = true;
                    if (!doorAudioSources[i].isPlaying) doorAudioSources[i].Play();
                }
            }
        }

        private void ResetAllAnomalies()
        {
            if (bedNumberText != null) bedNumberText.gameObject.SetActive(false);
            if (ceilingNumberText != null) ceilingNumberText.gameObject.SetActive(false);
            if (mirrorArrowObject != null) mirrorArrowObject.SetActive(false);
            if (paintingTransform != null) paintingTransform.localRotation = paintingBaseRotation;
            if (carpetRenderer != null) carpetRenderer.material.color = Color.gray;
            if (windowRenderer != null) windowRenderer.material.color = Color.white;
            if (ceilingLight != null) ceilingLight.color = defaultLightColor;
        }
    }
}

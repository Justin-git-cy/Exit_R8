// Assets/Scripts/Loop/LoopVariantData.cs
using UnityEngine;

namespace ExitR8.Loop
{
    public enum AnomalyType { None, PaintingTilt, ClockTime, CarpetPattern, DrawerAjar, Custom }

    [CreateAssetMenu(fileName = "LoopVariant", menuName = "ExitR8/Loop Variant")]
    public class LoopVariantData : ScriptableObject
    {
        [Header("Which of the 3 doors is safe this stage (0, 1, or 2)")]
        [Range(0, 2)] public int correctDoorIndex;

        [Header("The anomaly to apply this stage")]
        public AnomalyType anomalyType = AnomalyType.None;

        [Tooltip("Generic numeric knob: rotation degrees, clock hour offset, etc.")]
        public float anomalyValue;

        [Tooltip("Used for CarpetPattern anomalies.")]
        public Color anomalyColor = Color.white;

        [Header("Text shown when the player examines the anomalous object")]
        [TextArea(2, 4)] public string clueText;

        [Header("Design notes (not used at runtime)")]
        [TextArea(2, 4)] public string designerNotes;
    }
}

// Assets/Scripts/Loop/LoopVariantSet.cs
using UnityEngine;

namespace ExitR8.Loop
{
    [CreateAssetMenu(fileName = "LoopVariantSet", menuName = "ExitR8/Loop Variant Set")]
    public class LoopVariantSet : ScriptableObject
    {
        public LoopVariantData[] variants;

        public LoopVariantData Get(int stageIndex)
        {
            if (variants == null || variants.Length == 0) return null;
            int i = Mathf.Clamp(stageIndex, 0, variants.Length - 1);
            return variants[i];
        }
    }
}

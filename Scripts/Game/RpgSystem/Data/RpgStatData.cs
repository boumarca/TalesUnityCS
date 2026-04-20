using System;
using UnityEngine;

namespace Game.RpgSystem.Data
{
    [Serializable]
    public class RpgStatData
    {
        [field: SerializeField] public RpgStats StatType { get; private set; }
        [field: SerializeField] public int BaseValue { get; private set; }
        [field: SerializeField] public float GrowthValue { get; private set; }
    }
}

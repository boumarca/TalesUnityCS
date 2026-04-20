using System.Collections.Generic;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.RpgSystem.Data
{
    [CreateAssetMenu(fileName = "NewRpgItemData", menuName = "Game Data/RpgItemData")]
    public class RpgItemData : ScriptableObject
    {
        #region Serialized Fields and Properties
        [field: SerializeField] public ItemId Id { get; private set; }
        [field: SerializeField] public LocalizedString Name { get; private set; }
        [field: SerializeField] public LocalizedString Description { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [SerializeField] private ItemCategory[] _categories;
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite InfoImage { get; private set; }

        //TODO: Implement item effects. This value is temporary.
        [field: SerializeField] public float EffectValue { get; private set; }

        [field: SerializeField] public EquipSlot EquippableSlot { get; private set; }

        [SerializedTupleLabels("Stat", "Value")]
        [SerializeField] private SerializedTuple<RpgStats, int>[] _stats;

        [SerializedTupleLabels("Effect", "Level")]
        [SerializeField] private SerializedTuple<ItemEffectData, int>[] _defaultEffects;

        [SerializeField] private RpgElements[] _attackElements;

        [SerializedTupleLabels("Element", "Value Percent")]
        [SerializeField] private SerializedTuple<RpgElements, int>[] _defenseElements;
        #endregion

        #region Private Fields
        private Dictionary<RpgStats, int> _statsMap;
        private Dictionary<RpgElements, int> _defenseElementsMap;
        private Dictionary<ItemEffectData, int> _defaultEffectsMap;
        #endregion

        #region Public Properties
        public IReadOnlyCollection<ItemCategory> Categories => _categories;
        public IReadOnlyDictionary<RpgStats, int> Stats
        {
            get
            {
                _statsMap ??= _stats?.ToDictionary();
                return _statsMap;
            }
        }

        public IReadOnlyDictionary<ItemEffectData, int> DefaultEffects
        {
            get
            {
                _defaultEffectsMap ??= _defaultEffects?.ToDictionary();
                return _defaultEffectsMap;
            }
        }

        public IReadOnlyCollection<RpgElements> AttackElements => _attackElements;
        public IReadOnlyDictionary<RpgElements, int> DefenseElements
        {
            get
            {
                _defenseElementsMap ??= _defenseElements?.ToDictionary();
                return _defenseElementsMap;
            }
        }

        public bool IsKeyItem => Type == ItemType.KeyItem;
        public bool IsEquipment => EquippableSlot != EquipSlot.None;
        public bool IsConsummable => Type == ItemType.Consummable;
        #endregion

        #region Public Methods
        public int GetStatValue(RpgStats stat)
        {
            IReadOnlyDictionary<RpgStats, int> stats = Stats;
            if (stats != null && stats.TryGetValue(stat, out int value))
                return value;
            return 0;
        }
        #endregion
    }
}

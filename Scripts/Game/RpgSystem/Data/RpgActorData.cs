using System.Collections.Generic;
using System.Linq;
using Framework.Assertions;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.RpgSystem.Data
{
    [CreateAssetMenu(fileName = "NewRpgActorData", menuName = "Game Data/RpgActorData")]
    public class RpgActorData : ScriptableObject
    {
        #region Serialized Fields and Properties
        [field: SerializeField] public ActorId Id { get; private set; }
        [SerializeField] private LocalizedString _name;
        [field: SerializeField] public Sprite Headshot { get; private set; } 
        [field: SerializeField] public Sprite Portrait { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite Chara { get; private set; }
        [SerializeField] private RpgStatData[] _stats;
        [field: SerializeField] public int BaseAp {  get; private set; }
        [SerializeField] private List<ItemCategory> _equippableItemCategories;
        [SerializedTupleLabels("Equip Slot", "Default Item Id")]
        [SerializeField] private SerializedTuple<EquipSlot, ItemId>[] _defaultEquipment;
        #endregion

        #region Public Properties
        public LocalizedString Name => _name;
        public IReadOnlyCollection<RpgStatData> Stats => _stats;
        public IReadOnlyCollection<ItemCategory> EquippableCategories => _equippableItemCategories;
        public IReadOnlyDictionary<EquipSlot, ItemId> DefaultEquipment => _defaultEquipment?.ToDictionary();

        #endregion

        #region Public Methods
        public RpgStatData GetStat(RpgStats stat)
        {
            RpgStatData statData = Stats.FirstOrDefault(s => s.StatType == stat);
            AssertWrapper.IsNotNull(statData, $"Cannot find {stat} on actor {_name.GetLocalizedString()}. Make sure the value exists in the inspector.");
            return statData;
        }
        #endregion
    }
}

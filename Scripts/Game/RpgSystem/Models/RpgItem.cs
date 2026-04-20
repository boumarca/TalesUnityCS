using System.Collections.Generic;
using System.Linq;
using Framework.Extensions;
using Game.RpgSystem.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.RpgSystem.Models
{
    public class RpgItem
    {
        #region Constants
        public const int MaxEffectCount = 3;
        #endregion

        #region Private Fields
        private RpgItemData _data;
        private List<ItemEffect> _effects = new();
        #endregion

        #region Public Properties
        public ItemId Id => _data.Id;
        public LocalizedString Name => _data.Name;
        public LocalizedString Description => _data.Description;
        public ItemType Type => _data.Type;
        public IReadOnlyCollection<ItemCategory> Categories => _data.Categories; 
        public Sprite Icon => _data.Icon;
        public Sprite InfoImage => _data.InfoImage;
        public EquipSlot EquippableSlot => _data.EquippableSlot;
        public IReadOnlyCollection<ItemEffect> Effects => _effects;
        public IReadOnlyCollection<RpgElements> AttackElements => _data.AttackElements;
        public IReadOnlyDictionary<RpgElements, int> DefenseElements => _data.DefenseElements;
        public float EffectValue => _data.EffectValue;

        public bool IsKeyItem => _data.IsKeyItem;
        public bool IsEquipment => _data.IsEquipment;
        public bool IsConsummable => _data.IsConsummable;
        public long Timestamp { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of this item
        /// </summary>
        /// <param name="data"></param>
        public RpgItem(RpgItemData data) : this(data, null)
        {
        }

        /// <summary>
        /// Creates an instance of the item with the specified effects
        /// </summary>
        /// <param name="data"></param>
        /// <param name="effects"></param>
        public RpgItem(RpgItemData data, IReadOnlyCollection<ItemEffect> effects)
        {
            _data = data;
            if(effects != null && effects.Count > 0)
                _effects.AddRange(effects);
            else
                AddBaseEffects();
        }
        #endregion


        #region Public Methods
        public bool IsSameItem(RpgItem otherItem)
        {
            if (otherItem == null || otherItem.Id != Id || _effects.Count != otherItem._effects.Count)
                return false;

            foreach (ItemEffect effect in _effects)
            {
                if (otherItem._effects.None(e => e.IsSameEffect(effect)))
                    return false;
            }
            return true;
        }

        public int GetStatValue(RpgStats stat)
        {
            return _data.GetStatValue(stat) + ComputeEffectBoosts(stat);
        }

        public int CalculateOptimalValue()
        {
            if (Type == ItemType.Weapon)
                return GetStatValue(RpgStats.PhysicalAttack) + GetStatValue(RpgStats.MagicAttack);
            else if (Type is ItemType.ArmorBody or ItemType.ArmorHead or ItemType.ArmorArm)
                return GetStatValue(RpgStats.PhysicalDefense) + GetStatValue(RpgStats.MagicDefense);
            else if (Type == ItemType.Accessory && _data.Stats != null)
                return _data.Stats.Values.Sum();
            return 0;
        }

        public RpgItemSaveData SaveData()
        {
            RpgItemSaveData data = new()
            {
                ItemId = Id.Id,
                Timestamp = Timestamp
            };
            foreach (ItemEffect effect in _effects)
                data.Effects.Add(effect.SaveData());

            return data;
        }
        #endregion

        #region Private Methods
        private void AddBaseEffects()
        {
            IReadOnlyDictionary<ItemEffectData, int> defaultEffects = _data.DefaultEffects;
            if (defaultEffects == null)
                return;

            foreach (KeyValuePair<ItemEffectData, int> effect in _data.DefaultEffects)
            {
                ItemEffect itemEffect = new ItemEffect(effect.Key, effect.Value);
                _effects.Add(itemEffect);
            }
        }

        private int ComputeEffectBoosts(RpgStats stat)
        {
            //TODO: Make the system correctly when more effects are added.
            if (stat != RpgStats.PhysicalAttack)
                return 0;

            int boost = 0;
            foreach (ItemEffect effect in _effects)
            {
                if (effect.EffectType == EffectType.AttackUp)
                    boost += effect.EffectValue;
            }
            return boost;
        }
        #endregion
    }
}

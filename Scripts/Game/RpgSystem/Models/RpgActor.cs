using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.RpgSystem.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.RpgSystem.Models
{
    public class RpgActor
    {
        #region Constants
        private const int MaxLevel = 255;
        #endregion

        #region Private Fields
        private readonly RpgActorData _data;
        private readonly Dictionary<RpgStats, float> _stats = new();
        private readonly Dictionary<EquipSlot, RpgItem> _equipment = new();
        private int _currentHp;
        private int _currentMp;
        #endregion

        #region Public Properties
        public ActorId Id => _data.Id;
        public LocalizedString Name => _data.Name;
        public Sprite Portrait => _data.Portrait;
        public Sprite Headshot => _data.Headshot;
        public Sprite Icon => _data.Icon;
        public Sprite Chara => _data.Chara;
        public int Level => LevelInfo.Level;
        public LevelInfo LevelInfo { get; private set; }
        public int CurrentHp
        {
            get => _currentHp;
            set => _currentHp = Mathf.Clamp(value, 0, GetStatValue(RpgStats.MaxHp)); //TODO: Add death if 0
        }
        public int CurrentMp
        {
            get => _currentMp;
            set => _currentMp = Mathf.Clamp(value, 0, GetStatValue(RpgStats.MaxMp));
        }
        public int CurrentAp { get; private set; }
        public IReadOnlyDictionary<EquipSlot, ItemId> DefaultEquipment => _data.DefaultEquipment;
        public IReadOnlyCollection<RpgStats> Stats => _stats.Keys;
        public IReadOnlyCollection<EquipSlot> EquipSlots => _equipment.Keys;

        public bool IsFullHp => CurrentHp == GetStatValue(RpgStats.MaxHp);
        #endregion

        #region Constructors
        public RpgActor(RpgActorData data)
        {
            _data = data;
            LevelInfo = new LevelInfo(MaxLevel, ComputeExperienceForLevel);
            LevelInfo.OnLevelUpEvent += HandleOnLevelUp;
            InitializeStats();
            InitializeEquipSlots();
        }
        #endregion

        #region Public Methods
        public int GetStatValue(RpgStats stat)
        {
            int baseStat = Mathf.FloorToInt(_stats[stat]);
            int equipmentStats = _equipment.Values.Sum(item => item != null ? item.GetStatValue(stat) : 0);
            return baseStat + equipmentStats;
        }

        public RpgItem GetEquippedItem(EquipSlot slot)
        {
            return _equipment[slot];
        }

        public IReadOnlyCollection<RpgElements> GatherAttackElements()
        {
            HashSet<RpgElements> elements = new();
            foreach (RpgItem item in _equipment.Values)
            {
                if (item == null || item.AttackElements == null)
                    continue;

                foreach (RpgElements element in item.AttackElements)
                    elements.Add(element);
            }

            if(elements.Count <= 1)
                return elements;

            return elements.OrderBy(e => (int)e).ToList();
        }

        public IReadOnlyDictionary<RpgElements, int> GatherDefenseElements()
        {
            Dictionary<RpgElements, int> elements = new();
            foreach (RpgItem item in _equipment.Values)
            {
                if (item == null || item.DefenseElements == null)
                    continue;

                foreach (KeyValuePair<RpgElements, int> kvp in item.DefenseElements)
                {
                    if(elements.ContainsKey(kvp.Key))
                        elements[kvp.Key] += kvp.Value;
                    else
                        elements[kvp.Key] = kvp.Value;
                }
            }

            if (elements.Count <= 1)
                return elements;

            return elements.OrderBy(e => (int)e.Key).ToDictionary(e => e.Key, e => e.Value);
        }

        public void GainExperience(int amount)
        {
            LevelInfo.GainExperience(amount);
        }

        public void EquipItem(EquipSlot slot, RpgItem item)
        {
            _equipment[slot] = item;
        }

        public RpgItem UnequipItem(EquipSlot slot)
        {
            RpgItem equippedItem = _equipment[slot];
            _equipment[slot] = null;
            return equippedItem;
        }

        public bool CanEquip(RpgItem item)
        {
            foreach(ItemCategory category in item.Categories)
            {
                if(_data.EquippableCategories.Contains(category))
                    return true;
            }
            return false;
        }

        public bool CanEquip(RpgItemData item)
        {
            foreach (ItemCategory category in item.Categories)
            {
                if (_data.EquippableCategories.Contains(category))
                    return true;
            }
            return false;
        }

        public bool CanEquip(EquipSlot slot, RpgItem item)
        {
            if (!item.Categories.Contains(GetCategoryForSlot(slot)))
                return false;

            return CanEquip(item);
        }

        public bool IsEquipped(RpgItem item)
        {
            return GetEquippedItem(item.EquippableSlot)?.Id == item.Id;
        }

        /// <summary>
        /// Compare if the provided item is better than the equipped item in the specified slot.
        /// </summary>
        /// <param name="item">Item to test</param>
        /// <param name="slot">Slot to check</param>
        /// <returns></returns>
        public bool CompareEquipment(RpgItem item, EquipSlot slot)
        {
            RpgItem currentItem = GetEquippedItem(slot);
            if (currentItem == null)
                return true;

            return item.CalculateOptimalValue() > currentItem.CalculateOptimalValue();
        }

        public ActorSaveData SaveData()
        {
            Dictionary<string, string> statsSave = new();
            foreach (KeyValuePair<RpgStats,float> kvp in _stats)
                statsSave.Add(kvp.Key.ToString(), kvp.Value.ToString(CultureInfo.InvariantCulture));

            List<EquipmentSaveData> equipment = new();
            foreach (KeyValuePair<EquipSlot, RpgItem> kvp in _equipment)
            {
                EquipmentSaveData equipSlot = new EquipmentSaveData
                {
                    Slot = kvp.Key,
                    Item = kvp.Value?.SaveData()
                };
                equipment.Add(equipSlot);
            }

            return new ActorSaveData
            {
                ActorId = Id.Id,
                Level = LevelInfo.Level,
                TotalExperience = LevelInfo.TotalExperience,
                Stats = statsSave,
                EquippedItems = equipment
            };
        }

        public void LoadData(ActorSaveData saveData)
        {
            foreach (KeyValuePair<string, string> kvp in saveData.Stats)
            {
                bool keySuccess = Enum.TryParse(kvp.Key, out RpgStats statKey);
                bool valueSuccess = float.TryParse(kvp.Value, NumberStyles.None, CultureInfo.InvariantCulture, out float statValue);
                if(keySuccess && valueSuccess)
                    _stats[statKey] = statValue;
            }
            LevelInfo.LoadData(saveData.Level, saveData.TotalExperience);
        }
        #endregion

        #region Private Methods
        private void InitializeStats()
        {
            foreach (RpgStatData stat in _data.Stats)
                _stats.Add(stat.StatType, stat.BaseValue);
            CurrentHp = (int)_stats[RpgStats.MaxHp];
            CurrentMp = (int)_stats[RpgStats.MaxMp];
            CurrentAp = _data.BaseAp;
        }

        private void InitializeEquipSlots()
        {
            _equipment.Add(EquipSlot.Weapon, null);
            _equipment.Add(EquipSlot.ArmorBody, null);
            _equipment.Add(EquipSlot.ArmorHead, null);
            _equipment.Add(EquipSlot.ArmorArm, null);
            _equipment.Add(EquipSlot.Accessory, null);
        }

        private int ComputeExperienceForLevel(int level)
        {
            //Experience formula: x^3
            return Mathf.FloorToInt(Mathf.Pow(level, 3));
        }

        private void HandleOnLevelUp()
        {
            List<RpgStats> stats = _stats.Keys.ToList();
            foreach (RpgStats stat in stats)
            {
                int previousStat = (int)_stats[stat];
                _stats[stat] += _data.GetStat(stat).GrowthValue;
                int delta = (int)_stats[stat] - previousStat;
                if (stat == RpgStats.MaxHp)
                    CurrentHp += delta;
                else if (stat == RpgStats.MaxMp)
                    CurrentMp += delta;
            }
        }

        private static ItemCategory GetCategoryForSlot(EquipSlot slot)
        {
            return slot switch
            {
                EquipSlot.Weapon => ItemCategory.Weapon,
                EquipSlot.ArmorBody => ItemCategory.ArmorBody,
                EquipSlot.ArmorHead => ItemCategory.ArmorHead,
                EquipSlot.ArmorArm => ItemCategory.ArmorArm,
                EquipSlot.Accessory => ItemCategory.Accessory,
                _ => ItemCategory.Weapon
            };
        }
        #endregion
    }
}

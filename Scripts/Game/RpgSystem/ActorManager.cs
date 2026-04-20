using System;
using System.Collections.Generic;
using Framework.Assertions;
using Framework.Singleton;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.SaveSystem;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.RpgSystem
{
    public class ActorManager : GlobalSingleton<ActorManager>, ISaveable
    {
        #region Statics
        private static readonly Dictionary<RpgStats, int> s_projectedStatsBuffer = new();
        #endregion

        #region Serialized Fields
        [SerializeField] private ActorDatabase _actorDatabase;
        #endregion

        #region Private Fields
        private Dictionary<ActorId, RpgActor> _allActors = new();
        #endregion

        #region Public Properties
        public IReadOnlyCollection<ActorId> ActorIds => _allActors.Keys;
        #endregion

        #region ISaveable Implementation
        public IEnumerable<Type> SaveDataTypes => new[] { typeof(ActorsSaveData), typeof(ActorSaveData), typeof(Dictionary<string, string>), typeof(EquipmentSaveData) };
        public bool TryLoadData(SaveDataBase saveData)
        {
            if (saveData is not ActorsSaveData partySaveData)
                return false;

            InitializeFromSaveData(partySaveData);
            return true;
        }

        public SaveDataBase SaveData()
        {
            ActorsSaveData saveData = new();
            foreach (RpgActor actor in _allActors.Values)
                saveData.Actors.Add(actor.SaveData());

            return saveData;
        }
        #endregion

        #region Public Methods
        public void InitializeAsNew()
        {
            LoadActorDatabase();
            EquipDefaultEquipment();
        }

        public void GiveExperienceToActor(ActorId actorId, int amount)
        {
            RpgActor actor = GetActor(actorId);
            actor.GainExperience(amount);
        }

        public RpgActor GetActor(ActorId id)
        {
            return _allActors[id];
        }

        public IReadOnlyDictionary<RpgStats, int> ProjectNewEquipmentStats(RpgActor actor, EquipSlot slot, RpgItem newItem)
        {
            s_projectedStatsBuffer.Clear();
            RpgItem currentItem = actor.UnequipItem(slot);
            actor.EquipItem(slot, newItem);

            foreach (RpgStats stat in actor.Stats)
                s_projectedStatsBuffer[stat] = actor.GetStatValue(stat);
            
            actor.EquipItem(slot, currentItem);
            return s_projectedStatsBuffer;
        }

        public void EquipActor(RpgActor actor, EquipSlot slot, RpgItem item)
        {
            UnequipActor(actor, slot);
            InventoryManager.Instance.RemoveItem(item, 1);
            actor.EquipItem(slot, item);
        }

        public void UnequipActor(RpgActor actor, EquipSlot slot)
        {
            RpgItem equippedItem = actor.UnequipItem(slot);
            if (equippedItem != null)
                InventoryManager.Instance.AddBackItem(equippedItem, 1);
        }

        public void OptimizeEquipment(RpgActor actor)
        {
            List<EquipSlot> slotsKeys = ListPool<EquipSlot>.Get();
            slotsKeys.AddRange(actor.EquipSlots);
            foreach(EquipSlot equipSlot in slotsKeys)
            {
                RpgItem foundItem = InventoryManager.Instance.FindBestEquippableItem(actor, equipSlot);
                if (foundItem == null)
                    continue;
      
                if(actor.CompareEquipment(foundItem, equipSlot))
                    EquipActor(actor, equipSlot, foundItem);
            }
            ListPool<EquipSlot>.Release(slotsKeys);
        }

        public void UseItemOnActor(RpgActor actor, RpgItem item)
        {
            InventoryManager.Instance.RemoveItem(item, 1);
            //TODO: Have more effects than just HP
            actor.CurrentHp += Mathf.RoundToInt(actor.GetStatValue(RpgStats.MaxHp) * item.EffectValue);
        }
        #endregion

        #region Private Methods
        private void LoadActorDatabase()
        {
            _allActors.Clear();
            foreach (RpgActorData actorData in _actorDatabase)
                _allActors.Add(actorData.Id, new RpgActor(actorData));
        }

        private void EquipDefaultEquipment()
        {
            foreach(RpgActor actor in _allActors.Values)
            {
                if (actor.DefaultEquipment == null)
                    continue;

                foreach(KeyValuePair<EquipSlot, ItemId> equipment in actor.DefaultEquipment)
                {
                    RpgItem item = InventoryManager.Instance.MakeItemFromId(equipment.Value);
                    AssertWrapper.IsTrue(actor.CanEquip(item), $"{actor} should be able to equip {item}. Either change default equipment or review equippable categories.");
                    actor.EquipItem(equipment.Key, item);
                }
            }
        }

        private void InitializeFromSaveData(ActorsSaveData actorsSaveData)
        {
            LoadActorDatabase();
            foreach (ActorSaveData actorSaveData in actorsSaveData.Actors)
            {
                RpgActor actor = GetActor(new ActorId(actorSaveData.ActorId));
                actor.LoadData(actorSaveData);
                LoadEquipment(actorSaveData, actor);
            }
        }

        //Load equipment from here because actors don't have direct access to the inventory module.
        private static void LoadEquipment(ActorSaveData actorSaveData, RpgActor actor)
        {
            foreach (EquipmentSaveData equipment in actorSaveData.EquippedItems)
            {
                if (equipment.Item == null)
                    continue;

                RpgItem equippedItem = InventoryManager.Instance.LoadItem(equipment.Item);
                actor.EquipItem(equipment.Slot, equippedItem);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Assertions;
using Framework.Singleton;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.SaveSystem;
using UnityEngine;

namespace Game.RpgSystem
{
    public class InventoryManager : GlobalSingleton<InventoryManager>, ISaveable
    {
        #region Serialized Fields
        [SerializeField] private ItemDatabase _itemDatabase;
        [SerializeField] private ItemEffectDatabase _itemEffectDatabase;
        #endregion

        #region Private Fields
        private List<InventoryItem> _inventory = new();
        #endregion

        #region Events
        public event Action<InventoryItem> OnItemAddedEvent = delegate { };
        #endregion

        #region ISaveable Implementation
        public IEnumerable<Type> SaveDataTypes => new[] { typeof(InventorySaveData), typeof(InventoryItemSaveData), typeof(ItemEffectSaveData), typeof(RpgItemSaveData) };

        public bool TryLoadData(SaveDataBase saveData)
        {
            if (saveData is not InventorySaveData inventorySaveData)
                return false;

            InitializeFromSaveData(inventorySaveData);
            return true;
        }

        public SaveDataBase SaveData()
        {
            InventorySaveData saveData = new();
            foreach (InventoryItem item in _inventory)
                saveData.Inventory.Add(item.SaveData());

            return saveData; 
        }
        #endregion

        #region Public Methods
        public void InitializeAsNew()
        {
            _inventory.Clear();
        }

        /// <summary>
        /// Add to a specific item stack, or create one if it doesn't exists.
        /// Takes item effect in account when finding a stack.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        public void AddItem(RpgItem item, int quantity = 1)
        {
            InventoryItem inventoryItem = GetItemSlot(item);
            if (inventoryItem != null)
            {
                inventoryItem.AddQuantity(quantity);
            }
            else
            {
                inventoryItem = new InventoryItem(item, quantity);
                _inventory.Add(inventoryItem);
            }
            OnItemAddedEvent(inventoryItem);
        }

        /// <summary>
        /// Add back the item to the inventory.
        /// This keeps the item's timestamp when adding, to prevent treating it as a new item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        public void AddBackItem(RpgItem item, int quantity = 1)
        {
            InventoryItem inventoryItem = GetItemSlot(item);
            long currentTimestamp = item.Timestamp;
            if (inventoryItem != null)
            {
                inventoryItem.AddQuantity(quantity);
            }
            else
            {
                inventoryItem = new InventoryItem(item, quantity);
                _inventory.Add(inventoryItem);
            }
            inventoryItem.Item.Timestamp = currentTimestamp;
        }

        public void RemoveItem(RpgItem item, int quantity)
        {
            InventoryItem inventoryItem = GetItemSlot(item);
            inventoryItem.RemoveQuantity(quantity);
            if (inventoryItem.Quantity <= 0)
                _inventory.Remove(inventoryItem);
        }

        public void RemoveAllOfItem(RpgItem item)
        {
            InventoryItem inventoryItem = GetItemSlot(item);
            _inventory.Remove(inventoryItem);
        }

        /// <summary>
        /// Get quantity of all items with id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int GetItemQuantity(ItemId itemId)
        {
            return GetAllItemSlots(itemId).Sum(s => s.Quantity);
        }

        /// <summary>
        /// Get Quantity of this specific item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemQuantity(RpgItem item)
        {
            InventoryItem inventoryItem = GetItemSlot(item);
            return inventoryItem != null ? inventoryItem.Quantity : 0;
        }

        public bool HasItem(RpgItem item)
        {
            return GetItemQuantity(item) > 0;
        }

        /// <summary>
        /// Get the item stack of the specific item instance, taking effects into account.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InventoryItem GetItemSlot(RpgItem item)
        {
            return _inventory.Find(i => item.IsSameItem(i.Item));
        }

        public RpgItem MakeItemFromId(ItemId id, IReadOnlyCollection<ItemEffect> effects = null)
        {
            return new RpgItem(GetItemData(id), effects);
        }

        /// <summary>
        /// Create a new Collection with the Inventory items
        /// </summary>
        /// <param name="filter">Optional filtering by category</param>
        /// <param name="sortMode">Optional ordering of items</param>
        /// <returns></returns>
        public IEnumerable<InventoryItem> GetInventoryCopy(ItemType filter = ItemType.None)
        {
            return GetFilteredInventoryCopy(filter);
        }

        /// <summary>
        /// Create a new Collection containing the equippable items for the specified RpgActor and EquipSlot 
        /// </summary>
        /// <param name="actor">Filter items equippable by this actor</param>
        /// <param name="slot">Filter items equippable to this EquipSlot</param>
        /// <param name="filter">Optional filtering by category</param>
        /// <param name="sortMode">Optional ordering of items</param>
        /// <returns></returns>
        public IEnumerable<InventoryItem> GetEquippableItemsCopy(RpgActor actor, EquipSlot slot, ItemType filter = ItemType.None)
        {
            return GetFilteredInventoryCopy(filter).Where(item => actor.CanEquip(slot, item.Item));
        }

        /// <summary>
        /// Create a new Collection containing all the items for the specified ItemCategory 
        /// </summary>
        /// <param name="category">Filter items by this category tag</param>
        /// <param name="filter">Optional filtering by category</param>
        /// <param name="sortMode">Optional ordering of items</param>
        /// <returns></returns>
        public IEnumerable<InventoryItem> GetCategoryItemsCopy(ItemCategory category, ItemType filter = ItemType.None)
        {
            return GetFilteredInventoryCopy(filter).Where(item => item.Item.Categories.Contains(category));
        }

        /// <summary>
        /// Create a new Collection containing all the items for the specified ItemId
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public IEnumerable<InventoryItem> GetAllItemSlots(ItemId itemId)
        {
            return _inventory.Where(item => item.Item.Id == itemId);
        }

        public RpgItem FindBestEquippableItem(RpgActor actor, EquipSlot slot)
        {
            IEnumerable<InventoryItem> equippableItems = GetEquippableItemsCopy(actor, slot);
            RpgItem bestItem = null;
            int bestStats = 0;

            foreach (InventoryItem item in equippableItems)
            {
                RpgItem potentialItem = item.Item;
                int totalStats = potentialItem.CalculateOptimalValue();
                if (totalStats > bestStats)
                {
                    bestStats = totalStats;
                    bestItem = potentialItem;
                }
            }
            return bestItem;
        }

        public RpgItem LoadItem(RpgItemSaveData itemSaveData)
        {
            RpgItemData itemData = GetItemData((ItemId)itemSaveData.ItemId);

            List<ItemEffect> effects = new();
            foreach (ItemEffectSaveData effectSaveData in itemSaveData.Effects)
                effects.Add(new ItemEffect(_itemEffectDatabase.First(e => e.EffectType == effectSaveData.EffectType), effectSaveData.Level));

            RpgItem item = new(itemData, effects)
            {
                Timestamp = itemSaveData.Timestamp
            };
            return item;
        }
        #endregion

        #region Private Methods
        private void InitializeFromSaveData(InventorySaveData saveData)
        {
            InitializeAsNew();
            foreach (InventoryItemSaveData itemSaveData in saveData.Inventory)
                LoadInventoryItem(itemSaveData);
        }

        private void LoadInventoryItem(InventoryItemSaveData itemSaveData)
        {
            RpgItem item = LoadItem(itemSaveData.ItemSaveData);
            InventoryItem inventoryItem = new(item, itemSaveData.Quantity, item.Timestamp);
            _inventory.Add(inventoryItem);
        }

        private RpgItemData GetItemData(ItemId id)
        {
            RpgItemData item = _itemDatabase.FirstOrDefault(item => item.Id == id);
            AssertWrapper.IsNotNull(item, $"{id} doesn't exist in ItemDatabase");
            return item;
        }

        private IEnumerable<InventoryItem> GetFilteredInventoryCopy(ItemType filter = ItemType.None)
        {
            IEnumerable<InventoryItem> inventory = _inventory;
            inventory = FilterInventoryBy(inventory, filter);
            return inventory;
        }

        private static IEnumerable<InventoryItem> FilterInventoryBy(IEnumerable<InventoryItem> inventory, ItemType filter)
        {
            if(filter == ItemType.None)
                return inventory;
            return inventory.Where(item => item.Item.Type == filter);
        }
        #endregion
    }
}

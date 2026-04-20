using System;
using Game.RpgSystem.Data;

namespace Game.RpgSystem.Models
{
    public class InventoryItem
    {
        #region Public Properties
        public RpgItem Item { get; private set; }
        public int Quantity { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new item stack for this specific item with a quantity of 1.
        /// </summary>
        /// <param name="item"></param>
        public InventoryItem(RpgItemData item) : this(item, 1) { }

        /// <summary>
        /// Create a new item and stack for this item data with the specified quantity.
        /// </summary>
        /// <param name="item"></param>
        /// /// <param name="quantity"></param>
        public InventoryItem(RpgItemData item, int quantity) : this(new RpgItem(item), quantity) { }

        /// <summary>
        /// Create a new item stack for this specific item with the specified quantity.
        /// </summary>
        /// <param name="item"></param>
        /// /// <param name="quantity"></param>
        public InventoryItem(RpgItem item, int quantity) : this(item, quantity, DateTime.UtcNow.Ticks) { }

        /// <summary>
        /// Create a new item stack for this specific item with the specified quantity and timestamp.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        /// <param name="timestamp"></param>
        public InventoryItem(RpgItem item, int quantity, long timestamp)
        {
            Item = item;
            Quantity = quantity;
            Item.Timestamp = timestamp;
        }
        #endregion

        #region Public Methods
        public void AddQuantity(int amount)
        {
            Quantity += amount;
            Item.Timestamp = DateTime.UtcNow.Ticks;
        }

        public void RemoveQuantity(int amount)
        {
            Quantity -= amount;
        }

        public InventoryItemSaveData SaveData()
        {
            InventoryItemSaveData saveData = new()
            {
                ItemSaveData = Item.SaveData(),
                Quantity = Quantity
            };
            return saveData;
        }
        #endregion
    }
}

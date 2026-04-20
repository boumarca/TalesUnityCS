using System.Collections.Generic;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

namespace Jrpg.Debug
{
    public class InventoryDebugMenu : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ItemId _itemId;
        [SerializedTupleLabels("Effect Data", "Level")]
        [SerializeField] private SerializedTuple<ItemEffectData, int>[] _effectsToAdd; 
        [SerializeField] private int _itemCount;
        #endregion

        #region Public Properties
        public ItemId SelectedItem => _itemId;
        public int ItemCount => _itemCount;
        public IEnumerable<InventoryItem> Inventory => InventoryManager.Instance.GetInventoryCopy();
        #endregion

        #region Public Methods
        public void CheatAddItem()
        {
            List<ItemEffect> effects = new();
            int effectMax = Mathf.Min(_effectsToAdd.Length, RpgItem.MaxEffectCount);
            for (int i = 0; i < effectMax; i++)
                effects.Add(new ItemEffect(_effectsToAdd[i].v1, _effectsToAdd[i].v2));

            RpgItem item = InventoryManager.Instance.MakeItemFromId(_itemId, effects);
            InventoryManager.Instance.AddItem(item, _itemCount);
        }

        public void CheatRemoveQuantity(InventoryItem item)
        {
            InventoryManager.Instance.RemoveItem(item.Item, _itemCount);
        }

        public void CheatRemoveItem(InventoryItem item)
        {
            InventoryManager.Instance.RemoveAllOfItem(item.Item);
        }
        #endregion
    }
}

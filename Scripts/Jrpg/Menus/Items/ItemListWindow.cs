using System;
using System.Collections.Generic;
using System.Linq;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;

namespace Jrpg.Menus.Items
{
    public class ItemListWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ListView _listView;
        [SerializeField] private SortLabel _sortLabel;
        [SerializeField] private InputtableWindow _window;
        #endregion

        #region Private Fields
        private ItemType _currentFilter;
        #endregion

        #region Public Properties
        public ItemListEntry SelectedEntry => _listView.SelectedListEntry as ItemListEntry;
        public int EntryCount => _listView.EntryCount;
        public InputtableWindow Window => _window;
        public SortLabel SortLabel => _sortLabel;
        public ListView List => _listView;
        public RpgItem SelectedItem { get; private set; }
        #endregion

        #region Events
        public event Action<RpgItem> OnSelectedItemChangedEvent = delegate { };
        public event Action<object> OnItemConfirmedEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _listView.OnSelectedEntryChangedEvent += HandleOnSelectedEntryChanged;
        }

        private void OnDisable()
        {
            _listView.OnSelectedEntryChangedEvent -= HandleOnSelectedEntryChanged;
        }
        #endregion

        #region Public Methods
        public void RefreshItemList(bool keepIndex = false)
        {
            IEnumerable<InventoryItem> filteredList = InventoryManager.Instance.GetInventoryCopy(_currentFilter);
            RefreshItemList(filteredList, keepIndex);
        }

        public void RefreshItemList(IEnumerable<InventoryItem> itemList, bool keepIndex = false, bool ignoreSorting = false)
        {
            IEnumerable<InventoryItem> sortedList = itemList;
            if(!ignoreSorting)
                sortedList = SortInventoryBy(itemList, _sortLabel.SortMode);
            _listView.PopulateList(sortedList, OnItemConfirmedEvent, keepIndex);
        }

        public void RefreshItemList(RpgActor actor, EquipSlot slot, bool keepIndex = false)
        {
            //To prevent null ref if item list is refreshed before the actor is set to the slot window. 
            if (actor == null)
                return;

            IEnumerable<InventoryItem> filteredList = InventoryManager.Instance.GetEquippableItemsCopy(actor, slot, _currentFilter);
            RefreshItemList(filteredList, keepIndex);
        }        

        public void ChangeFilter(ItemType filter)
        {
            _currentFilter = filter;
        }
        #endregion

        #region Private Methods
        private void HandleOnSelectedEntryChanged(ListEntry entry)
        {
            SelectedItem = null;
            if (entry != null)
                SelectedItem = ((entry as ItemListEntry).Data as InventoryItem)?.Item;

            OnSelectedItemChangedEvent(SelectedItem);
        }

        private static IEnumerable<InventoryItem> SortInventoryBy(IEnumerable<InventoryItem> list, SortingMode sortMode)
        {
            return sortMode switch
            {
                SortingMode.None => list,
                SortingMode.Id => list.OrderBy(item => item.Item.Id), //TODO: Use ItemDatabase index and presort when adding items
                SortingMode.Newest => list.OrderByDescending(item => item.Item.Timestamp),
                SortingMode.Alphabetical => list.OrderBy(item => item.Item.Name.GetLocalizedString()),
                SortingMode.PhysicalAttack => list.OrderByDescending(item => item.Item.GetStatValue(RpgStats.PhysicalAttack)),
                SortingMode.PhysicalDefense => list.OrderByDescending(item => item.Item.GetStatValue(RpgStats.PhysicalDefense)),
                SortingMode.MagicAttack => list.OrderByDescending(item => item.Item.GetStatValue(RpgStats.MagicAttack)),
                SortingMode.MagicDefense => list.OrderByDescending(item => item.Item.GetStatValue(RpgStats.MagicDefense)),
                SortingMode.Agility => list.OrderByDescending(item => item.Item.GetStatValue(RpgStats.Agility)),
                SortingMode.Quantity => list.OrderByDescending(item => item.Quantity),
                _ => list,
            };
        }
        #endregion
    }
}

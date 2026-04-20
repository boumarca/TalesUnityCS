using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Inputs;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Pool;

namespace Jrpg.Menus.Items
{
    public class ItemsMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private InputtableWindow _inputListWindow;
        [SerializeField] private InputtableWindow _quickItemInputWindow;
        [SerializeField] private ItemListWindow _itemListWindow;
        [SerializeField] private ItemInfoView _itemInfoView;
        [SerializeField] private QuickItemWindow _quickItemWindow;
        [SerializeField] private ItemTabView _itemTabList;
        [Header("Game Data")]
        [SerializeField] private int _maxNewItems;
        [SerializeField] private List<SortingMode> _sortOptions;
        [Header("Localization")]
        [SerializeField] private LocalizedString _discardItemConfirmation;
        [SerializeField] private LocalizedString _useItemText;
        [SerializeField] private LocalizedString _equipItemText;
        #endregion

        #region Private Fields
        private bool _isNewTab;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _itemListWindow.OnSelectedItemChangedEvent += HandleSelectedItemChanged;
            _itemListWindow.OnItemConfirmedEvent += HandleOnItemConfirmed;
            _inputListWindow.OnActivatedEvent += RefreshInputs;
            _quickItemWindow.OnActorConfirmedEvent += HandleOnQuickItemActorConfirmed;
            _itemTabList.OnNewTabSelectedEvent += HandleOnNewTabSelected;
            _itemTabList.OnFilteredTabSelectedEvent += HandleOnFilteredTabSelected;
        }

        private void OnDisable()
        {
            _itemListWindow.OnSelectedItemChangedEvent -= HandleSelectedItemChanged;
            _itemListWindow.OnItemConfirmedEvent -= HandleOnItemConfirmed;
            _inputListWindow.OnActivatedEvent -= RefreshInputs;
            _quickItemWindow.OnActorConfirmedEvent -= HandleOnQuickItemActorConfirmed;
            _itemTabList.OnNewTabSelectedEvent -= HandleOnNewTabSelected;
            _itemTabList.OnFilteredTabSelectedEvent -= HandleOnFilteredTabSelected;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            _itemListWindow.SortLabel.ChangeSortOptions(_sortOptions);
            _itemTabList.SelectFirstTab();
            return Task.CompletedTask;
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnDiscardItemPerformed()
        {
            DisplayDiscardConfirmation();
        }

        public void UIOnSortItemsPerformed()
        {
            _itemListWindow.SortLabel.ChangeSortMode();
            _itemListWindow.RefreshItemList(true);
        }

        public void UIOnChangeTabPerformed(InputAction.CallbackContext context)
        {
            _itemTabList.ChangeTab((int)context.ReadValue<float>());
        }

        public void UIOnChangePagePerformed(InputAction.CallbackContext context)
        {
            _itemListWindow.List.ChangePage(context.ReadValue<float>());
        }

        public void UIOnChangeInfoPerformed()
        {
            _itemInfoView.ChangePage();
        }

        public void UIOnCancelQuickItemPerformed()
        {
            CloseQuickItemWindow();
        }
        #endregion

        #region Private Methods
        private void CreateFilteredItemList(ItemType newFilter)
        {
            _isNewTab = false;
            _itemListWindow.ChangeFilter(newFilter);
            _itemListWindow.RefreshItemList();
            RefreshSortInput();
            RefreshChangePageInput();
        }

        private void CreateNewItemsList()
        {
            List<InventoryItem> itemList = ListPool<InventoryItem>.Get();
            IEnumerable<InventoryItem> inventory = InventoryManager.Instance.GetInventoryCopy().OrderByDescending(item => item.Item.Timestamp);
            itemList.AddRange(inventory.Take(_maxNewItems));
            _itemListWindow.RefreshItemList(itemList, ignoreSorting: true);
            ListPool<InventoryItem>.Release(itemList);
        }

        private void HandleSelectedItemChanged(RpgItem selectedItem)
        {
             _itemInfoView.Refresh(selectedItem);
            RefreshConfirmInput();
            RefreshDiscardInput();
            RefreshInfoToggleInput();
        }

        private void HandleOnItemConfirmed(object itemData)
        {
            if (itemData is not InventoryItem inventoryItem)
                return;

            RpgItem item = inventoryItem.Item;
            OpenItemUseWindow(item);
        }

        private void HandleOnQuickItemActorConfirmed(RpgActor actor, RpgItem item)
        {
            //TODO: Split these into two functions if it ever grows too big.
            if (item.IsEquipment)
            {
                ActorManager.Instance.EquipActor(actor, item.EquippableSlot, item);
                _itemListWindow.RefreshItemList(true);
            }
            else if (item.IsConsummable)
            {
                ActorManager.Instance.UseItemOnActor(actor, item);
                if (InventoryManager.Instance.HasItem(_itemListWindow.SelectedItem))
                    _itemListWindow.SelectedEntry.RefreshQuantity();
                else
                    _itemListWindow.List.RemoveSelectedEntry();
            }

            if (!InventoryManager.Instance.HasItem(_itemListWindow.SelectedItem))
                CloseQuickItemWindow();
        }

        private void HandleOnNewTabSelected()
        {
            _isNewTab = true;
            CreateNewItemsList();
            RefreshSortInput();
            RefreshChangePageInput();
        }

        private void HandleOnFilteredTabSelected(ItemType filter)
        {
            CreateFilteredItemList(filter);
        }

        private void OpenItemUseWindow(RpgItem item)
        {
            ChangeWindowFocus(_quickItemInputWindow);
            _quickItemWindow.DisplayWindow(item);
        }

        private void CloseQuickItemWindow()
        {
            _itemListWindow.List.SelectCurrent();
            _quickItemWindow.Close();
            ChangeWindowFocus(_inputListWindow);
        }

        private void DisplayDiscardConfirmation()
        {
            DisplayConfirmationPopup(_discardItemConfirmation, DiscardItem);
        }

        private void DiscardItem()
        {
            InventoryManager.Instance.RemoveItem(_itemListWindow.SelectedItem, 1);
            if (InventoryManager.Instance.HasItem(_itemListWindow.SelectedItem))
            {
                ItemListEntry selectedItemEntry = _itemListWindow.SelectedEntry;
                selectedItemEntry.RefreshQuantity();
                EventSystem.current.SetSelectedGameObject(selectedItemEntry.gameObject);
            }
            else
            {
                _itemListWindow.List.RemoveSelectedEntry();
            }
        }

        private void RefreshInputs()
        {
            RefreshConfirmInput();
            RefreshDiscardInput();
            RefreshSortInput();
            RefreshInfoToggleInput();
            RefreshChangePageInput();
        }

        private void RefreshConfirmInput()
        {
            RpgItem selectedItem = _itemListWindow.SelectedItem;
            if (selectedItem == null || selectedItem.IsKeyItem || selectedItem.Type == ItemType.Material)
            {
                DisableConfirmInput();
                return;
            }

            EnableConfirmInput();
            GameInputs.MenusActions actions = InputManager.Instance.InputActions.Menus;
            if (selectedItem.IsEquipment)
                UIManager.Instance.ChangeInputLabel(actions.Confirm, _equipItemText);
            else
                UIManager.Instance.ChangeInputLabel(actions.Confirm, _useItemText);
        }

        private void RefreshDiscardInput()
        {
            RpgItem selectedItem = _itemListWindow.SelectedItem;
            RefreshInput(InputManager.Instance.InputActions.ItemMenu.DiscardItem, selectedItem != null && !selectedItem.IsKeyItem);
        }

        private void RefreshSortInput()
        {
            bool canSort = !_isNewTab;
            _itemListWindow.SortLabel.DisplaySortLabel(canSort);
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.Sort, canSort);
        }

        private void RefreshInfoToggleInput()
        {
            RpgItem selectedItem = _itemListWindow.SelectedItem;
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangeInfo, selectedItem != null && !selectedItem.IsKeyItem);
        }

        private void RefreshChangePageInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangePage, _itemListWindow.EntryCount > 1);
        }
        #endregion

    }
}

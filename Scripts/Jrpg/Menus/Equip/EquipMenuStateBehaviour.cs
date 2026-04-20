using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Inputs;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using Jrpg.Menus.Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jrpg.Menus.Equip
{
    public class EquipMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private EquipSlotWindow _equipSlotWindow;
        [SerializeField] private PartyMembersTabView _partyMembersTabView;
        [SerializeField] private ItemListWindow _itemListWindow;
        [SerializeField] private ItemInfoView _itemInfoView;
        [SerializeField] private EquipStatInfoView _equipStatInfoView;
        [SerializeField] private InputtableWindow _equipSlotInputWindow;
        [SerializeField] private InputtableWindow _itemListInputWindow;

        [SerializeField] private List<SortingMode> _weaponSortOptions;
        [SerializeField] private List<SortingMode> _armorSortOptions;
        [SerializeField] private List<SortingMode> _accessorySortOptions;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _partyMembersTabView.OnTabChangedEvent += HandleTabChanged;
            _equipSlotWindow.OnSelectedSlotChangedEvent += HandleSelectedSlotChanged;
            _itemListWindow.OnSelectedItemChangedEvent += HandleSelectedItemChanged;
            _itemListWindow.OnItemConfirmedEvent += HandleOnItemConfirmed;
            _itemListInputWindow.OnActivatedEvent += RefreshItemWindowInputs;
        }

        private void OnDisable()
        {
            _partyMembersTabView.OnTabChangedEvent -= HandleTabChanged;
            _equipSlotWindow.OnSelectedSlotChangedEvent -= HandleSelectedSlotChanged;
            _itemListWindow.OnSelectedItemChangedEvent -= HandleSelectedItemChanged;
            _itemListWindow.OnItemConfirmedEvent -= HandleOnItemConfirmed;
            _itemListInputWindow.OnActivatedEvent -= RefreshItemWindowInputs;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            _partyMembersTabView.PopulateTabs();
            _partyMembersTabView.SelectFirstTab();
            _itemListInputWindow.Deactivate();
            RefreshChangeTabInput();
            return Task.CompletedTask;
        }
        #endregion

        #region Unity UI Event Methods
        public void UIOnChangeTabPerformed(InputAction.CallbackContext context)
        {
            _partyMembersTabView.ChangeTab((int)context.ReadValue<float>());
        }

        public void UIOnChangePagePerformed(InputAction.CallbackContext context)
        {
            _itemListWindow.List.ChangePage(context.ReadValue<float>());
        }

        public void UIOnItemWindowCanceledPerformed()
        {
            ExitItemWindow();
            RefreshItemInfosWindow();
        }

        public void UIOnSortItemsPerformed()
        {
            SortItemList();
        }

        public void UIOnEquipSlotClicked()
        {
            SelectItemWindow();
        }

        public void UIOnRemovePerformed()
        {
            RemoveItemFromSlot();
        }

        public void UIOnChangeInfoPerformed()
        {
            _itemInfoView.ChangePage();
        }

        public void UIOnBestGearPerformed()
        {
            EquipBestGear();            
        }
        #endregion

        #region Private Methods
        private void RefreshItemWindow(bool keepIndex = false)
        {
            _itemListWindow.RefreshItemList(_equipSlotWindow.Actor, _equipSlotWindow.SelectedSlotType, keepIndex);
        }

        private void RefreshItemInfosWindow()
        {
            _itemInfoView.Refresh(_equipSlotWindow.SelectedSlotItem);
        }

        private void SelectItemWindow()
        {
            if (_itemListWindow.EntryCount == 0)
                return;

            ChangeWindowFocus(_itemListInputWindow);
            _itemListWindow.List.SelectIndex(0);
            _equipStatInfoView.Show();
        }

        private void ExitItemWindow()
        {
            ChangeWindowFocus(_equipSlotInputWindow);
            _itemListWindow.List.ClearSelection();
            _equipStatInfoView.Hide();
            RefreshEquipSlotWindowInputs();
        }

        private void RemoveItemFromSlot()
        {
            UnequipActor(_equipSlotWindow.SelectedSlotType);
            _equipSlotWindow.ChangeSelectedSlotItem(null);
            _itemInfoView.Refresh(null);
            RefreshItemWindow();
            RefreshEquipSlotWindowInputs();
        }

        private void EquipBestGear()
        {
            ActorManager.Instance.OptimizeEquipment(_equipSlotWindow.Actor);
            _equipSlotWindow.Refresh();
            RefreshItemInfosWindow();
            RefreshItemWindow();
            RefreshEquipSlotWindowInputs();
        }

        private void SortItemList()
        {
            _itemListWindow.SortLabel.ChangeSortMode();
            RefreshItemWindow(true);
        }

        private void HandleTabChanged(RpgActor actor)
        {
            _equipSlotWindow.ChangeActor(actor);
            _equipStatInfoView.ChangeActor(actor);
            RefreshItemInfosWindow();
            RefreshItemWindow();
            RefreshEquipSlotWindowInputs();
        }

        private void HandleSelectedSlotChanged(EquipSlotEntry entry)
        {
            _itemListWindow.ChangeFilter(entry.SlotItemType);
            _itemListWindow.SortLabel.ChangeSortOptions(GetSortOptions());
            RefreshItemInfosWindow();
            RefreshItemWindow();
            RefreshEquipSlotWindowInputs();
        }

        private void HandleSelectedItemChanged(RpgItem selectedItem)
        {
            if (selectedItem == null)
                return;

            IReadOnlyDictionary<RpgStats, int> projectedStats = ActorManager.Instance.ProjectNewEquipmentStats(_equipSlotWindow.Actor, _equipSlotWindow.SelectedSlotType, selectedItem);
            _equipStatInfoView.RefreshProjectedStats(projectedStats);
            _itemInfoView.Refresh(selectedItem);
            RefreshInfoToggleInput();
        }

        private void HandleOnItemConfirmed(object itemData)
        {
            if (itemData is not InventoryItem inventoryItem)
                return;

            EquipActor(inventoryItem.Item);
            ExitItemWindow();
            RefreshItemWindow();
        }

        private void EquipActor(RpgItem itemToEquip)
        {
            ActorManager.Instance.EquipActor(_equipSlotWindow.Actor, _equipSlotWindow.SelectedSlotType, itemToEquip);
            _equipSlotWindow.ChangeSelectedSlotItem(itemToEquip);
        }

        private void UnequipActor(EquipSlot slot)
        {
            ActorManager.Instance.UnequipActor(_equipSlotWindow.Actor, slot);
        }

        private List<SortingMode> GetSortOptions()
        {
            return _equipSlotWindow.SelectedSlotType switch
            {
                EquipSlot.Weapon => _weaponSortOptions,
                EquipSlot.ArmorBody or
                EquipSlot.ArmorHead or
                EquipSlot.ArmorArm => _armorSortOptions,
                EquipSlot.Accessory => _accessorySortOptions,
                _ => _weaponSortOptions
            };
        }

        private void RefreshEquipSlotWindowInputs()
        {
            RefreshRemoveInput();
            RefreshInfoToggleInput();
        }

        private void RefreshItemWindowInputs()
        {
            RefreshChangePageInput();
            RefreshInfoToggleInput();
        }

        private void RefreshRemoveInput()
        {
            bool enableCondition = _equipSlotWindow.SelectedSlotType != EquipSlot.Weapon && _equipSlotWindow.SelectedSlotItem != null;
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.Remove, enableCondition);
        }

        private void RefreshChangePageInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangePage, _itemListWindow.EntryCount > 1);
        }

        private void RefreshChangeTabInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangeTab, _partyMembersTabView.Count > 1);
        }

        private void RefreshInfoToggleInput()
        {
            bool enableCondition = _itemListWindow.SelectedItem != null || _equipSlotWindow.SelectedSlotItem != null;
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangeInfo, enableCondition);
        }
        #endregion
    }
}

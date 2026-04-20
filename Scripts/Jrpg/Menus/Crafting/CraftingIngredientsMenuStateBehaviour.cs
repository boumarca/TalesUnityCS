using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Inputs;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.StateStack;
using Jrpg.Menus.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace Jrpg.Menus.Crafting
{
    public class CraftingIngredientsMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private IngredientSelectionWindow _ingredientsWindow;
        [SerializeField] private ItemListWindow _itemListWindow;
        [SerializeField] private EffectListWindow _effectListWindow;
        [SerializeField] private ItemInfoView _itemInfoView;
        [SerializeField] private EffectInfoView _effectInfoView;
        [SerializeField] private List<SortingMode> _sortOptions;
        [SerializeField] private StateData _effectStateData;
        [SerializeField] private LocalizedString _cancelMessage;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _ingredientsWindow.OnSelectedSlotChangedEvent += HandleOnSelectedSlotChanged;
            _ingredientsWindow.OnSelectedSlotConfirmedEvent += HandleOnSelectedSlotConfirmed;
            _ingredientsWindow.OnContinueButtonSelectedEvent += HandleOnContinueButtonSelected;
            _itemListWindow.OnSelectedItemChangedEvent += HandleOnSelectedItemChanged;
            _itemListWindow.OnItemConfirmedEvent += HandleOnItemConfirmed;
            _itemListWindow.Window.OnActivatedEvent += RefreshItemWindowInputs;
            _effectListWindow.OnSelectedEffectChangedEvent += HandleOnSelectedEffectChanged;
        }

        private void OnDisable()
        {
            _ingredientsWindow.OnSelectedSlotChangedEvent -= HandleOnSelectedSlotChanged;
            _ingredientsWindow.OnSelectedSlotConfirmedEvent -= HandleOnSelectedSlotConfirmed;
            _ingredientsWindow.OnContinueButtonSelectedEvent -= HandleOnContinueButtonSelected;
            _itemListWindow.OnSelectedItemChangedEvent -= HandleOnSelectedItemChanged;
            _itemListWindow.OnItemConfirmedEvent -= HandleOnItemConfirmed;
            _itemListWindow.Window.OnActivatedEvent -= RefreshItemWindowInputs;
            _effectListWindow.OnSelectedEffectChangedEvent -= HandleOnSelectedEffectChanged;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            CreateItemModel(payload as CraftingRecipeData);
            _ingredientsWindow.InitializeSlots();
            _itemListWindow.SortLabel.ChangeSortOptions(_sortOptions);
            _itemListWindow.Window.Deactivate();
            return Task.CompletedTask;
        }

        public override void OnResumeState()
        {
            base.OnResumeState();
            RefreshIngredientsSlotWindowInputs();
        }
        #endregion

        #region Unity UI Event Methods
        public void UIOnItemWindowCanceledPerformed()
        {
            ExitItemWindow();
            RefreshItemInfosWindow();
        }

        public void UIOnEffectWindowCanceledPerformed()
        {
            ExitEffectWindow();
            RefreshItemInfosWindow();
        }

        public void UIOnCancelMenuPerformed()
        {
            if(_ingredientsWindow.SelectedSlot != null)
                DisplayCancelPopup();
            else
                _ingredientsWindow.BackToLastSelectedSlot();
        }

        public void UIOnRemovePerformed()
        {
            RemoveIngredient();
        }

        public void UIOnSortPerformed()
        {
            SortItemList();
        }

        public void UIOnChangeInfoPerformed()
        {
            _itemInfoView.ChangePage();
        }

        public void UIOnChangePagePerformed(InputAction.CallbackContext context)
        {
            _itemListWindow.List.ChangePage(context.ReadValue<float>());
        }

        public void UIOnDisplayEffectsPerformed()
        {
            DisplayEffectList();
        }

        public void UIOnContinueClicked()
        {
            if (CraftingManager.Instance.CraftingItemModel.HasEmptySlot())
                return;

            StateStackManager.Instance.PushState(_effectStateData);
        }
        #endregion

        #region Private Methods
        private void CreateItemModel(CraftingRecipeData recipe)
        {
            CraftingManager.Instance.CreateCraftingModel(recipe);
        }

        private void SelectItemWindow()
        {
            ChangeWindowFocus(_itemListWindow.Window);
            _itemListWindow.List.SelectIndex(0);
        }

        private void ExitItemWindow()
        {
            ChangeWindowFocus(_ingredientsWindow.Window);
            _itemListWindow.List.ClearSelection();
            RefreshIngredientsSlotWindowInputs();
        }

        private void RefreshItemList(bool keepIndex = false)
        {
            IngredientData selectedIngredientSlot = _ingredientsWindow.SelectedSlot.Ingredient;
            IEnumerable<InventoryItem> itemCollection = CraftingManager.CollectCompatibleItems(selectedIngredientSlot);
            _itemListWindow.RefreshItemList(itemCollection, keepIndex);
        }

        private void RefreshItemInfosWindow()
        {
            RpgItem item = _ingredientsWindow.SelectedSlot != null ? _ingredientsWindow.SelectedSlot.Item : null;
            _itemInfoView.Refresh(item);
            _itemInfoView.RefreshEffectCompatibility(CraftingManager.Instance.CraftingItemModel.Type);
        }

        private void AssignIngredient(RpgItem item)
        {
            _ingredientsWindow.AssignIngredient(item);
            ExitItemWindow();
            RefreshItemList();
            _ingredientsWindow.SelectNextEmptySlot();
        }        

        private void RemoveIngredient()
        {
            _ingredientsWindow.RemoveIngredient();
            _itemInfoView.Refresh(null);
            RefreshItemList();
            RefreshIngredientsSlotWindowInputs();
        }

        private void SortItemList()
        {
            _itemListWindow.SortLabel.ChangeSortMode();
            RefreshItemList(true);
        }

        private void DisplayEffectList()
        {
            ChangeWindowFocus(_effectListWindow.Window);
            _effectListWindow.RefreshEffectList(CraftingManager.Instance.CraftingItemModel.InheritedEffects);
            _effectListWindow.Show();
            _effectInfoView.Show();
            _itemInfoView.Hide();
            DisableConfirmInput();
        }

        private void ExitEffectWindow()
        {
            ChangeWindowFocus(_ingredientsWindow.Window);
            _effectListWindow.Hide();
            _effectInfoView.Hide();
            _itemInfoView.Show();
            EnableConfirmInput();
            RefreshIngredientsSlotWindowInputs();
        }

        private void DisplayCancelPopup()
        {
            DisplayConfirmationPopup(_cancelMessage, OnConfirm);
            return;

            void OnConfirm()
            {
                CraftingManager.Instance.CancelCurrentCrafting();
                CloseMenu();
            }
        }

        private void HandleOnSelectedSlotChanged(IngredientSlotEntry ingredientSlotEntry)
        {
            RefreshItemList();
            RefreshItemInfosWindow();
            RefreshIngredientsSlotWindowInputs();
        }

        private void HandleOnSelectedSlotConfirmed(IngredientSlotEntry ingredientSlotEntry)
        {
            SelectItemWindow();
        }

        private void HandleOnContinueButtonSelected()
        {
            _itemListWindow.RefreshItemList(Enumerable.Empty<InventoryItem>());
            _itemInfoView.Refresh(null);
            RefreshIngredientsSlotWindowInputs();
        }

        private void HandleOnSelectedItemChanged(RpgItem selectedItem)
        {
            if (selectedItem == null)
                return;

            _itemInfoView.Refresh(selectedItem);
            _itemInfoView.RefreshEffectCompatibility(CraftingManager.Instance.CraftingItemModel.Type);
            RefreshInfoToggleInput();
        }

        private void HandleOnItemConfirmed(object itemData)
        {
            if (itemData is not InventoryItem item)
                return;

            AssignIngredient(item.Item);
        }

        private void HandleOnSelectedEffectChanged(EffectListEntry entry)
        {
            ItemEffect effect = entry != null ? entry.Data as ItemEffect : null;
            _effectInfoView.Refresh(effect);
        }

        private void RefreshIngredientsSlotWindowInputs()
        {
            RefreshConfirmInput();
            RefreshRemoveInput();
            RefreshInfoToggleInput();
        }

        private void RefreshConfirmInput()
        {
            if(_ingredientsWindow.SelectedSlot != null)
            {
                EnableConfirmInput();
                return;
            }

            if (CraftingManager.Instance.CraftingItemModel.HasEmptySlot())
                DisableConfirmInput();
            else
                EnableConfirmInput();
        }


        private void RefreshItemWindowInputs()
        {
            RefreshChangePageInput();
            RefreshInfoToggleInput();
        }

        private void RefreshRemoveInput()
        {
            bool enableCondition = _ingredientsWindow.IsItemSelected();
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.Remove, enableCondition);
        }

        private void RefreshChangePageInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangePage, _itemListWindow.EntryCount > 1);
        }

        private void RefreshInfoToggleInput()
        {
            bool enableCondition = _itemListWindow.SelectedItem != null || _ingredientsWindow.IsItemSelected();
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangeInfo, enableCondition);
        }
        #endregion
    }
}

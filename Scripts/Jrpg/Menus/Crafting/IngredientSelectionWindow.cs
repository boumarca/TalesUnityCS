using System;
using Framework.Extensions;
using Game.CraftingSystem;
using Game.CraftingSystem.Models;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class IngredientSelectionWindow : MonoBehaviour
    {
        #region Serialized Field
        [SerializeField] private IngredientSlotEntry[] _ingredientSlotEntries;
        [SerializeField] private InputtableWindow _window;
        [SerializeField] private Button _continueButton;
        #endregion

        #region Private Fields
        private GameObject _currentSelection;
        private IngredientSlotEntry _lastSelectedSlot;
        #endregion

        #region Public Properties
        public IngredientSlotEntry SelectedSlot { get; private set; }
        public InputtableWindow Window => _window;
        #endregion

        #region Events
        public event Action<IngredientSlotEntry> OnSelectedSlotChangedEvent = delegate { };
        public event Action<IngredientSlotEntry> OnSelectedSlotConfirmedEvent = delegate { };
        public event Action OnContinueButtonSelectedEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _window.OnActivatedEvent += HandleOnActivated;
            foreach (IngredientSlotEntry entry in _ingredientSlotEntries)
            {
                entry.OnSlotSelectedEvent += HandleOnSlotSelected;
                entry.OnSlotConfirmedEvent += HandleOnSlotConfirmed;
            }
        }

        private void OnDisable()
        {
            _window.OnActivatedEvent -= HandleOnActivated;
            foreach (IngredientSlotEntry entry in _ingredientSlotEntries)
            {
                entry.OnSlotSelectedEvent -= HandleOnSlotSelected;
                entry.OnSlotConfirmedEvent -= HandleOnSlotConfirmed;
            }
        }
        #endregion

        #region Public Methods
        public void InitializeSlots()
        {
            FillIngredientsSlots();
            RefreshContinueButton();
            EventSystem.current.SetSelectedGameObject(_ingredientSlotEntries[0].gameObject);
        }        

        public void AssignIngredient(RpgItem item)
        {
            int slotIndex = GetSlotIndex();
            CraftingManager.Instance.AssignIngredientToSlot(slotIndex, item);
            SelectedSlot.SetItem(item, GetActualIngredientCount(item));
            SelectedSlot.DisplayItemSlot();
            RefreshContinueButton();
        }

        public void RemoveIngredient()
        {
            int slotIndex = GetSlotIndex();
            CraftingManager.Instance.EmptyIngredientSlot(slotIndex);
            SelectedSlot.EmptySlot();
            RefreshContinueButton();
        }

        public bool IsItemSelected()
        {
            return SelectedSlot != null && SelectedSlot.HasItem;
        }

        public void SelectNextEmptySlot()
        {
            int currentIndex = GetSlotIndex();
            for (int i = 1; i < _ingredientSlotEntries.Length; i++)
            {
                int newIndex = (currentIndex + i) % _ingredientSlotEntries.Length;
                IngredientSlotEntry slot = _ingredientSlotEntries[newIndex];
                if (!slot.gameObject.activeSelf)
                    continue;

                if (slot.IsEmpty)
                {
                    EventSystem.current.SetSelectedGameObject(slot.gameObject);
                    return;
                }
            }

            EventSystem.current.SetSelectedGameObject(_continueButton.gameObject);
        }

        public void BackToLastSelectedSlot()
        {
            EventSystem.current.SetSelectedGameObject(_lastSelectedSlot.gameObject);
        }
        #endregion

        #region Unity UI Event Methods
        public void UIOnContinueButtonSelected()
        {
            SelectedSlot = null;
            _currentSelection = _continueButton.gameObject;
            OnContinueButtonSelectedEvent();
        }
        #endregion

        #region Private Methods
        private void FillIngredientsSlots()
        {
            CraftingItem itemModel = CraftingManager.Instance.CraftingItemModel;
            for (int i = 0; i < _ingredientSlotEntries.Length; i++)
            {
                IngredientSlotEntry entry = _ingredientSlotEntries[i];
                if (i >= itemModel.IngredientCount)
                {
                    entry.gameObject.SetActive(false);
                    continue;
                }
                entry.gameObject.SetActive(true);
                entry.SetIngredient(itemModel.GetIngredientDataForSlot(i));
                entry.DisplayEmptySlot();
            }
        }

        private int GetSlotIndex()
        {
            return _ingredientSlotEntries.IndexOf(SelectedSlot);
        }

        private static int GetActualIngredientCount(RpgItem item)
        {
            int inventoryCount = InventoryManager.Instance.GetItemQuantity(item);
            int ingredientCount = CraftingManager.Instance.CraftingItemModel.GetAssignCount(item);
            return inventoryCount + ingredientCount;
        }

        private void RefreshContinueButton()
        {
            bool canCraft = !CraftingManager.Instance.CraftingItemModel.HasEmptySlot();
            ColorBlock colors = _continueButton.colors;
            _continueButton.targetGraphic.color = canCraft ? colors.normalColor : colors.disabledColor;
        }

        private void HandleOnActivated()
        {
            //Null check to avoid trying to select something before the window content is initialized.
            if (_currentSelection == null)
                return;

            EventSystem.current.SetSelectedGameObject(_currentSelection);
        }

        private void HandleOnSlotSelected(IngredientSlotEntry selectedSlot)
        {
            if (SelectedSlot == selectedSlot)
                return;

            SelectedSlot = selectedSlot;
            _lastSelectedSlot = SelectedSlot;
            _currentSelection = SelectedSlot.gameObject;
            OnSelectedSlotChangedEvent(SelectedSlot);
        }

        private void HandleOnSlotConfirmed(IngredientSlotEntry selectedSlot)
        {
            OnSelectedSlotConfirmedEvent(selectedSlot);
        }
        #endregion
    }
}

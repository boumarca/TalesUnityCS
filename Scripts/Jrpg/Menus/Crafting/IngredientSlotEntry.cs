using System;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;

namespace Jrpg.Menus.Crafting
{
    public class IngredientSlotEntry : MonoBehaviour, ISelectHandler
    {
        #region Serialized Fields
        [SerializeField] private Checkbox _checkbox;
        [SerializeField] private IngredientEntry _ingredient;
        [SerializeField] private LocalizeStringEvent _ingredientTypeText;
        #endregion

        #region Public Properties
        public bool IsEmpty => Item == null;
        public bool HasItem => !IsEmpty;
        public IngredientData Ingredient { get; private set; }
        public RpgItem Item { get; private set; }
        #endregion

        #region Events
        public event Action<IngredientSlotEntry> OnSlotSelectedEvent = delegate { };
        public event Action<IngredientSlotEntry> OnSlotConfirmedEvent = delegate { };
        #endregion

        #region ISelectHandler Implementation
        public void OnSelect(BaseEventData eventData)
        {
            OnSlotSelectedEvent(this);
        }
        #endregion

        #region Public Methods
        public void SetIngredient(IngredientData data)
        {
            Ingredient = data;
            _ingredientTypeText.StringReference = Ingredient.Name;
        }

        public void SetItem(RpgItem item, int ownedQuantity)
        {
            Item = item;
            if(Item != null)
                _ingredient.SetAssignedItem(item, ownedQuantity);
        }

        public void EmptySlot()
        {
            SetItem(null, 0);
            DisplayEmptySlot();
        }

        public void DisplayEmptySlot()
        {
            _checkbox.SetChecked(false);
            _ingredient.gameObject.SetActive(false);
            _ingredientTypeText.gameObject.SetActive(true);            
        }

        public void DisplayItemSlot()
        {
            _checkbox.SetChecked(true);
            _ingredient.gameObject.SetActive(true);
            _ingredientTypeText.gameObject.SetActive(false);            
        }
        #endregion

        #region Unity UI Event Methods
        public void UIOnSlotConfirmed()
        {
            OnSlotConfirmedEvent(this);
        }
        #endregion
    }
}

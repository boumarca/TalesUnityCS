using System.Globalization;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class IngredientEntry : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image _itemIcon;
        [SerializeField] private LocalizeStringEvent _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemNameTextMeshPro;
        [SerializeField] private TextMeshProUGUI _ownedQuantityText;
        [SerializeField] private TextMeshProUGUI _requiredQuantityText;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _notEnoughColor;
        #endregion

        #region Public Methods
        public void SetIngredientInfo(IngredientData ingredient, int ownedQuantity)
        {
            _itemIcon.gameObject.SetActive(!ingredient.IsCategory);
            if (!ingredient.IsCategory)
                _itemIcon.sprite = ingredient.Item.Icon;

            _itemNameText.StringReference = ingredient.Name;
            DisplayQuantities(ownedQuantity, ingredient.Quantity);

            Color textColor = ownedQuantity >= ingredient.Quantity ? _normalColor : _notEnoughColor;
            _itemNameTextMeshPro.color = textColor;
            _ownedQuantityText.color = textColor;
        }

        public void SetAssignedItem(RpgItem item, int ownedQuantity)
        {
            _itemIcon.sprite = item.Icon;
            _itemIcon.gameObject.SetActive(true);
            _itemNameText.StringReference = item.Name;
            DisplayQuantities(ownedQuantity, 1);
        }
        #endregion

        #region Private Methods
        private void DisplayQuantities(int owned, int required)
        {
            _ownedQuantityText.text = owned.ToString(CultureInfo.InvariantCulture);
            _requiredQuantityText.text = required.ToString(CultureInfo.InvariantCulture);
        }
        #endregion
    }
}

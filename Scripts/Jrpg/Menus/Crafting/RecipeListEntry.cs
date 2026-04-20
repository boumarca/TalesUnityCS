using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.RpgSystem;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class RecipeListEntry : ListEntry
    {
        #region Serialized Fields
        [SerializeField] private LocalizeStringEvent _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemNameTextMeshPro;
        [SerializeField] private TextMeshProUGUI _itemQuantityText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _cannotCraftColor;
        #endregion

        #region Private Fields
        private CraftingRecipeData _recipeData;
        #endregion

        #region MenuListEntry Implementation
        protected override void OnInitialize()
        {
            InitializeItem(Data as CraftingRecipeData);
            RefreshQuantity();
            RefreshCraftStatus();
        }
        #endregion

        #region Public Methods
        public void InitializeItem(CraftingRecipeData data)
        {
            _recipeData = data;
            _itemNameText.StringReference = _recipeData.Item.Name;
            _itemIcon.sprite = _recipeData.Item.Icon;            
#if UNITY_EDITOR
            name = _recipeData.Item.Name.GetLocalizedString();
#endif
        }

        public void HideQuantity()
        {
            _itemQuantityText.gameObject.SetActive(false);
        }

        public void RefreshCraftStatus()
        {
            Color textColor = CraftingManager.Instance.CanCraftRecipe(_recipeData) ? _normalColor : _cannotCraftColor;
            _itemNameTextMeshPro.color = textColor;
            _itemQuantityText.color = textColor;
        }
        #endregion

        #region Private Methods
        private void RefreshQuantity()
        {
            int quantity = InventoryManager.Instance.GetItemQuantity(_recipeData.Item.Id);
            _itemQuantityText.text = $": {quantity}";
        }
        #endregion
    }
}

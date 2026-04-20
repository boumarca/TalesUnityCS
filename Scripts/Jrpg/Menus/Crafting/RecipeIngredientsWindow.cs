using System.Globalization;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class RecipeIngredientsWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private GameObject _content;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private LocalizeStringEvent _recipeNameText;
        [SerializeField] private TextMeshProUGUI _recipeLevelText;
        [SerializeField] private IngredientEntry[] _ingredients;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _notEnoughColor;
        #endregion

        #region Private Fields
        private CraftingRecipeData _recipeData;
        #endregion

        #region Public Methods
        public void ChangeRecipe(CraftingRecipeData recipe)
        {
            _recipeData = recipe;
            if(_recipeData == null)
            {
                HideInfos();
                return;
            }
            ShowInfos();
            DisplayRecipeItem();
            DisplayRecipeLevel();
            DisplayIngredients();
        }
        #endregion

        #region Private Methods
        private void HideInfos()
        {
            _content.SetActive(false);
        }

        private void ShowInfos()
        {
            _content.SetActive(true);
        }

        private void DisplayRecipeItem()
        {
            _itemIcon.sprite = _recipeData.Item.Icon;
            _recipeNameText.StringReference = _recipeData.Item.Name;
        }

        private void DisplayRecipeLevel()
        {
            _recipeLevelText.text = _recipeData.RecipeLevel.ToString(CultureInfo.InvariantCulture);
            _recipeLevelText.color = CraftingManager.Instance.HasRequiredCraftingLevel(_recipeData) ? _normalColor : _notEnoughColor;
        }

        private void DisplayIngredients()
        {
            int index = 0;
            foreach(IngredientData ingredient in _recipeData.Ingredients)
            {
                _ingredients[index].SetIngredientInfo(ingredient, CraftingManager.GetOwnedIngredientQuantity(ingredient));
                _ingredients[index].gameObject.SetActive(true);
                index++;
            }

            for (; index < _ingredients.Length; index++)
                _ingredients[index].gameObject.SetActive(false);
        }
        #endregion
    }
}

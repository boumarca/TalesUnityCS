using System;
using System.Collections.Generic;
using System.Linq;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Data;
using Game.UI;
using UnityEngine;

namespace Jrpg.Menus.Crafting
{
    public class RecipeListWindow : MonoBehaviour
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
        public InputtableWindow Window => _window;
        public int EntryCount => _listView.EntryCount;
        public SortLabel SortLabel => _sortLabel;
        public ListView List => _listView;
        public CraftingRecipeData SelectedRecipe { get; private set; }
        #endregion

        #region Events
        public event Action<CraftingRecipeData> OnSelectedRecipeChangedEvent = delegate { };
        public event Action<object> OnRecipeConfirmedEvent = delegate { };
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
            IEnumerable<CraftingRecipeData> recipeList = CraftingManager.Instance.LearnedRecipes;

            recipeList = FilterInventoryBy(recipeList, _currentFilter);
            recipeList = SortInventoryBy(recipeList, _sortLabel.SortMode);
            _listView.PopulateList(recipeList, OnRecipeConfirmedEvent, keepIndex);
        }

        public void ChangeFilter(ItemType filter)
        {
            _currentFilter = filter;
        }
        #endregion

        #region Private Methods
        private static IEnumerable<CraftingRecipeData> FilterInventoryBy(IEnumerable<CraftingRecipeData> recipes, ItemType filter)
        {
            if (filter == ItemType.None)
                return recipes;
            return recipes.Where(item => item.Item.Type == filter);
        }

        private IEnumerable<CraftingRecipeData> SortInventoryBy(IEnumerable<CraftingRecipeData> recipes, SortingMode sortMode)
        {
            return sortMode switch
            {
                SortingMode.None => recipes,
                SortingMode.Id => recipes.OrderBy(recipe => recipe.Item.Id), //TODO: Use ItemDatabase index and presort when adding items
                SortingMode.Alphabetical => recipes.OrderBy(recipe => recipe.Item.Name.GetLocalizedString()),
                SortingMode.CraftingLevel => recipes.OrderBy(recipe => recipe.RecipeLevel),
                SortingMode.CraftableStatus => recipes.OrderByDescending(recipe => CraftingManager.Instance.CanCraftRecipe(recipe)),
                _ => recipes,
            };
        }

        private void HandleOnSelectedEntryChanged(ListEntry entry)
        {
            SelectedRecipe = null;
            if (entry != null)
                SelectedRecipe = entry.Data as CraftingRecipeData;

            OnSelectedRecipeChangedEvent(SelectedRecipe);
        }
        #endregion
    }
}

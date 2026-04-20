using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Inputs;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Data;
using Game.StateStack;
using Jrpg.Menus.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace Jrpg.Menus.Crafting
{
    public class CraftingRecipeMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private CraftingLevelWindow _craftingLevelWindow;
        [SerializeField] private RecipeListWindow _recipeListWindow;
        [SerializeField] private ItemTabView _categoryTabs;
        [SerializeField] private RecipeInfoView _recipeInfoView;
        [SerializeField] private RecipeIngredientsWindow _recipeIngredientsWindow;
        [SerializeField] private ResultPopupHandler _resultsHandler;
        [SerializeField] private NewRecipesWindow _newRecipesWindow;
        [SerializeField] private List<SortingMode> _sortOptions;

        [SerializeField] private StateData _ingredientsState;
        [SerializeField] private LocalizedString _craftingConfirmationMessage;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _categoryTabs.OnFilteredTabSelectedEvent += HandleOnFilteredTabSelected;
            _recipeListWindow.OnSelectedRecipeChangedEvent += HandleOnRecipeChanged;
            _recipeListWindow.OnRecipeConfirmedEvent += HandleOnRecipeConfirmed;
            _recipeListWindow.Window.OnActivatedEvent += RefreshInputs;
            _resultsHandler.OnResultsWindowClosedEvent += HandleOnResultWindowClosed;
            _newRecipesWindow.OnCloseEvent += HandleOnNewRecipesWindowClosed;
        }

        private void OnDisable()
        {
            _categoryTabs.OnFilteredTabSelectedEvent -= HandleOnFilteredTabSelected;
            _recipeListWindow.OnSelectedRecipeChangedEvent -= HandleOnRecipeChanged;
            _recipeListWindow.OnRecipeConfirmedEvent -= HandleOnRecipeConfirmed;
            _recipeListWindow.Window.OnActivatedEvent -= RefreshInputs;
            _resultsHandler.OnResultsWindowClosedEvent -= HandleOnResultWindowClosed;
            _newRecipesWindow.OnCloseEvent -= HandleOnNewRecipesWindowClosed;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            _craftingLevelWindow.Refresh();
            _recipeListWindow.SortLabel.ChangeSortOptions(_sortOptions);
            _categoryTabs.SelectFirstTab();
            if (CraftingManager.Instance.HasNewRecipes())
                DisplayNewRecipesPopup();
            return Task.CompletedTask;
        }

        public override void OnResumeState()
        {
            base.OnResumeState();
            DisplayNextScreenOnReturn();
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnSortRecipeListPerformed()
        {
            _recipeListWindow.SortLabel.ChangeSortMode();
            _recipeListWindow.RefreshItemList(true);
        }

        public void UIOnChangeInfoPerformed()
        {
            _recipeInfoView.ChangePage();
        }

        public void UIOnChangeTabPerformed(InputAction.CallbackContext context)
        {
            _categoryTabs.ChangeTab((int)context.ReadValue<float>());
        }

        public void UIOnChangePagePerformed(InputAction.CallbackContext context)
        {
            _recipeListWindow.List.ChangePage(context.ReadValue<float>());
        }

        public void UIOnSimpleCraftPerformed()
        {
            CraftingRecipeData recipe = _recipeListWindow.SelectedRecipe;
            if (recipe == null)
                return;

            if (CraftingManager.Instance.CanCraftRecipe(recipe))
            {
                _craftingConfirmationMessage.Arguments = new List<object> { recipe.Item.Name.GetLocalizedString() };
                DisplayConfirmationPopup(_craftingConfirmationMessage, OnConfirm);
            }

            return;

            void OnConfirm()
            {
                CraftingManager.Instance.AutoCraftRecipe(recipe);
                ChangeWindowFocus(_resultsHandler.Window);
                _resultsHandler.DisplayCraftingResults(recipe);
            }
        }
        #endregion

        #region Private Methods
        private void DisplayNextScreenOnReturn()
        {
            if (CraftingManager.Instance.HasNewRecipes())
                DisplayNewRecipesPopup();
            else
                DisplayRecipeScreen();
        }

        private void DisplayRecipeScreen()
        {
            ChangeWindowFocus(_recipeListWindow.Window);
            _craftingLevelWindow.Refresh();
            _recipeListWindow.RefreshItemList(true);
        }

        private void DisplayNewRecipesPopup()
        {
            ChangeWindowFocus(_newRecipesWindow.Window);
            _newRecipesWindow.Show();
            _newRecipesWindow.PopulateList(CraftingManager.Instance.NewRecipes);
            CraftingManager.Instance.MarkNewRecipesAsLearned();
            _craftingLevelWindow.Refresh();
            _recipeListWindow.RefreshItemList(true);
        }

        private void HandleOnFilteredTabSelected(ItemType filter)
        {
            _recipeListWindow.ChangeFilter(filter);
            _recipeListWindow.RefreshItemList();
            RefreshChangePageInput();
        }

        private void HandleOnRecipeChanged(CraftingRecipeData recipe)
        {
            _recipeInfoView.Refresh(recipe);
            _recipeIngredientsWindow.ChangeRecipe(recipe);
            RefreshInputs();
        }

        private void HandleOnResultWindowClosed()
        {
            DisplayNextScreenOnReturn();
        }

        private void HandleOnNewRecipesWindowClosed()
        {
            DisplayRecipeScreen();
        }

        private void HandleOnRecipeConfirmed(object data)
        {
            CraftingRecipeData recipe = data as CraftingRecipeData;
            if (recipe == null)
                return;

            if (CraftingManager.Instance.CanCraftRecipe(recipe))
                StateStackManager.Instance.PushState(_ingredientsState, recipe);
        }

        private void RefreshInputs()
        {
            RefreshConfirmInput();
            RefreshSimpleCraftInput();
            RefreshInfoToggleInput();
            RefreshChangePageInput();
        }

        private void RefreshConfirmInput()
        {
            if(_recipeListWindow.SelectedRecipe != null && CraftingManager.Instance.CanCraftRecipe(_recipeListWindow.SelectedRecipe))
                EnableConfirmInput();
            else
                DisableConfirmInput();
        }

        private void RefreshSimpleCraftInput()
        {
            bool enableCondition = _recipeListWindow.SelectedRecipe != null && CraftingManager.Instance.CanCraftRecipe(_recipeListWindow.SelectedRecipe);
            RefreshInput(InputManager.Instance.InputActions.CraftingMenu.SimpleCraft, enableCondition);
        }

        private void RefreshInfoToggleInput()
        {
            CraftingRecipeData selectedRecipe = _recipeListWindow.SelectedRecipe;
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangeInfo, selectedRecipe != null && selectedRecipe.Item.IsEquipment);
        }

        private void RefreshChangePageInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangePage, _recipeListWindow.EntryCount > 1);
        }
        #endregion
    }
}

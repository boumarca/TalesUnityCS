using System.Threading.Tasks;
using Framework.Inputs;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.CraftingSystem.Models;
using Game.RpgSystem.Models;
using Game.StateStack;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jrpg.Menus.Crafting
{
    public class CraftingEffectsMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private EffectListWindow _effectListWindow;
        [SerializeField] private EffectInfoView _effectInfoView;
        [SerializeField] private TextMeshProUGUI _effectsCountText;
        [SerializeField] private PartyMembersTabView _partyMembersTabView;
        [SerializeField] private CraftingSkillsView _craftingSkillsView;
        [SerializeField] private ResultPopupHandler _resultsHandler;
        [SerializeField] private StateData _recipeListStateData;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _effectListWindow.OnSelectedEffectChangedEvent += HandleOnSelectedEffectChanged;
            _effectListWindow.OnSelectedEffectConfirmedEvent += HandleOnSelectedEffectConfirmed;
            _resultsHandler.OnResultsWindowClosedEvent += HandleOnResultWindowClosed;
            _partyMembersTabView.OnTabChangedEvent += HandleTabChanged;
        }

        private void OnDisable()
        {
            _effectListWindow.OnSelectedEffectChangedEvent -= HandleOnSelectedEffectChanged;
            _effectListWindow.OnSelectedEffectConfirmedEvent -= HandleOnSelectedEffectConfirmed;
            _resultsHandler.OnResultsWindowClosedEvent -= HandleOnResultWindowClosed;
            _partyMembersTabView.OnTabChangedEvent -= HandleTabChanged;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            _partyMembersTabView.PopulateTabs();
            _partyMembersTabView.SelectFirstTab();
            _craftingSkillsView.DisplayCraftingLevel(CraftingManager.Instance.CraftingLevel);
            RefreshEffectWindow();
            RefreshChangeTabInput();
            return Task.CompletedTask;
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnCraftItemClicked()
        {
            CraftItem();
        }

        public void UIOnCancelMenuPerformed()
        {
            if(_effectListWindow.IsCraftButtonSelected)
                _effectListWindow.BackToLastEffect();
            else
                BackToIngredientsMenu();
        }

        public void UIOnChangeTabPerformed(InputAction.CallbackContext context)
        {
            _partyMembersTabView.ChangeTab((int)context.ReadValue<float>());
        }
        #endregion

        #region Private Methods       
        private void TransferEffect(ItemEffect effect)
        {
            CraftingItem itemModel = CraftingManager.Instance.CraftingItemModel;
            if (itemModel.IsEffectSelected(effect))
                itemModel.DeselectEffect(effect);
            else
                itemModel.SelectEffect(effect);

            bool isChecked = itemModel.IsEffectSelected(effect);
            _effectListWindow.SelectedEntry.SetChecked(isChecked);
            RefreshEffectCount();
            if(isChecked)
                _effectListWindow.SelectNextEffect();
        }

        private void RefreshEffectWindow()
        {
            _effectListWindow.RefreshEffectList(CraftingManager.Instance.CraftingItemModel.InheritedEffects, true);
            RefreshEffectCount();
        }

        private void RefreshEffectCount()
        {
            int effectCount = CraftingManager.Instance.CraftingItemModel.SelectedEffects.Count;
            _effectsCountText.text = $"{effectCount}/{RpgItem.MaxEffectCount}";
        }

        private void CraftItem()
        {
            CraftingRecipeData recipe = CraftingManager.Instance.CraftingItemModel.Recipe;
            CraftingManager.Instance.CraftItemModel();
            ChangeWindowFocus(_resultsHandler.Window);
            _resultsHandler.DisplayCraftingResults(recipe);
        }

        private void BackToIngredientsMenu()
        {
            CraftingManager.Instance.ClearActor();
            CraftingManager.Instance.CraftingItemModel.DeselectAllEffects();
            CloseMenu();
        }

        private void HandleOnSelectedEffectChanged(EffectListEntry entry)
        {
            ItemEffect effect = entry != null ? entry.Data as ItemEffect : null;
            _effectInfoView.Refresh(effect);
        }

        private void HandleOnSelectedEffectConfirmed(object obj)
        {
            if (obj is not ItemEffect effect)
                return;

            TransferEffect(effect);
        }

        private void HandleOnResultWindowClosed()
        {
            CraftingManager.Instance.ClearActor();
            StateStackManager.Instance.ReturnToPreviousState(_recipeListStateData);
        }

        private void HandleTabChanged(RpgActor actor)
        {
            _craftingSkillsView.ChangeActor(actor);
            CraftingManager.Instance.ChangeActor(actor);
            RefreshEffectWindow();
        }

        private void RefreshChangeTabInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangeTab, _partyMembersTabView.Count > 1);
        }
        #endregion
    }
}

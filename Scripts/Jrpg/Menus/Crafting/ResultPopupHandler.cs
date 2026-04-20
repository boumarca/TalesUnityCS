using System;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;

namespace Jrpg.Menus.Crafting
{
    public class ResultPopupHandler : MonoBehaviour
    {
        [SerializeField] private CraftingResultWindow _craftingResultWindow;
        [SerializeField] private InputtableWindow _window;

        public InputtableWindow Window => _window;

        public event Action OnResultsWindowClosedEvent = delegate { };

        private void OnEnable()
        {
            _craftingResultWindow.OnCloseEvent += OnResultsWindowClosedEvent;
        }

        private void OnDisable()
        {
            _craftingResultWindow.OnCloseEvent -= OnResultsWindowClosedEvent;
        }

        public void DisplayCraftingResults(CraftingRecipeData recipe)
        {
            AwardCraftingExperience(recipe);
            DisplayResultPopup(recipe);
        }

        private void AwardCraftingExperience(CraftingRecipeData recipe)
        {
            LevelInfo craftingLevelInfo = CraftingManager.Instance.LevelInfo;
            _craftingResultWindow.SetPreviousXpValues(craftingLevelInfo);
            int xpReceived = CraftingManager.Instance.ComputeRecipeExperience(recipe);
            _craftingResultWindow.SetGainedXp(xpReceived);
            CraftingManager.Instance.GainExperience(xpReceived);
            _craftingResultWindow.SetTargetXpValues(craftingLevelInfo);
        }

        private void DisplayResultPopup(CraftingRecipeData recipe)
        {
            _craftingResultWindow.Show();
            _craftingResultWindow.SetItem(recipe);
            _craftingResultWindow.SetCraftingBonus(CraftingManager.Instance.SkillResults);
            _craftingResultWindow.StartXpAnimation();
        }
    }
}

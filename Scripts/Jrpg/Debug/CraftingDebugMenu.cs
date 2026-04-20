using System.Collections.Generic;
using System.Linq;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.CraftingSystem.Models;
using Game.RpgSystem.Models;
using UnityEngine;

namespace Jrpg.Debug
{
    public class CraftingDebugMenu : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private int _cheatValue;
        #endregion

        #region Public Properties
        public int CheatValue => _cheatValue;
        public int CraftingLevel => CraftingManager.Instance.CraftingLevel;
        public int TotalExperience => CraftingManager.Instance.LevelInfo.TotalExperience;
        public IReadOnlyCollection<CraftingRecipeData> LearnedRecipes => CraftingManager.Instance.LearnedRecipes;
        public RpgActor SelectedActor => CraftingManager.Instance.CraftingActor;
        public CraftingItem CurrentItemModel => CraftingManager.Instance.CraftingItemModel;
        public bool IsCraftingStarted { get; private set; }
        public CraftingRecipeData SelectedRecipe { get; set; }
        #endregion

        #region Public Methods
        public int RemainingExperienceToNextLevel()
        {
            return CraftingManager.Instance.LevelInfo.RemainingExperienceToNextLevel();
        }

        public int GetOwnedIngredientQuantity(IngredientData ingredient)
        {
            return CraftingManager.GetOwnedIngredientQuantity(ingredient);
        }

        public void AutoCraft()
        {
            CraftingManager.Instance.AutoCraftRecipe(SelectedRecipe);
            AwardCraftingExperience();
        }

        public void StartCrafting()
        {
            CraftingManager.Instance.CreateCraftingModel(SelectedRecipe);
            IsCraftingStarted = true;
        }

        public void CancelCrafting()
        {
            CraftingManager.Instance.CancelCurrentCrafting();
            IsCraftingStarted = false;
        }

        public bool CanCraftRecipe(CraftingRecipeData recipe)
        {
            return CraftingManager.Instance.CanCraftRecipe(recipe);
        }

        public void AssignIngredientToSlot(int slotIndex, RpgItem ingredient)
        {
            if(CurrentItemModel.GetIngredientInSlot(slotIndex) != ingredient)
                CraftingManager.Instance.AssignIngredientToSlot(slotIndex, ingredient);
        }

        public void EmptyIngredientSlot(int slotIndex)
        {
            if (CurrentItemModel.GetIngredientInSlot(slotIndex) != null)
                CraftingManager.Instance.EmptyIngredientSlot(slotIndex);
        }

        public void SetSelectedActor(RpgActor actor)
        {
            if(SelectedActor != actor)
                CraftingManager.Instance.ChangeActor(actor);
        }

        public bool CanPerformCrafting()
        {
            return !CurrentItemModel.HasEmptySlot();
        }

        public void PerformCrafting()
        {
            CraftingManager.Instance.CraftItemModel();
            AwardCraftingExperience();
            CraftingManager.Instance.ClearActor();
            IsCraftingStarted = false;
        }

        public IReadOnlyList<InventoryItem> GetCompatibleItems(IngredientData ingredient)
        {
            return CraftingManager.CollectCompatibleItems(ingredient).ToList();
        }

        public IReadOnlyCollection<CraftingSkillData> GetActorCraftingSkills()
        {
            return CraftingManager.Instance.GetActorCraftingSkills(SelectedActor.Id);
        }

        public void CheatGiveCraftingExperience()
        {
            if(_cheatValue > 0)
                CraftingManager.Instance.GainExperience(_cheatValue);
        }
        #endregion

        #region Private Methods
        private void AwardCraftingExperience()
        {
            int xpReceived = CraftingManager.Instance.ComputeRecipeExperience(SelectedRecipe);
            UnityEngine.Debug.Log($"Received {xpReceived} crafting experience");
            CraftingManager.Instance.GainExperience(xpReceived);
            CraftingManager.Instance.MarkNewRecipesAsLearned();
        }
        #endregion
    }
}

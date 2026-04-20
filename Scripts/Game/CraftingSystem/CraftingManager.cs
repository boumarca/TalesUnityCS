using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Singleton;
using Game.CraftingSystem.Data;
using Game.CraftingSystem.Models;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.SaveSystem;
using UnityEngine;

namespace Game.CraftingSystem
{
    public class CraftingManager : GlobalSingleton<CraftingManager>, ISaveable
    {
        #region Serialized Fields
        [SerializeField] private int _maxLevel = 50;
        [SerializeField] private CraftingRecipeDatabase _allRecipes;
        [SerializeField] private ItemEffectDatabase _allEffects;
        [SerializeField] private CraftingSkillsMap _craftingSkillsMap;
        #endregion

        #region Private Fields
        private readonly List<CraftingRecipeData> _learnedRecipes = new();
        private readonly List<CraftingRecipeData> _newRecipes = new();
        private readonly List<CraftingSkillResult> _skillResults = new();
        #endregion

        #region Public Properties
        public IReadOnlyCollection<CraftingRecipeData> LearnedRecipes => _learnedRecipes;
        public IReadOnlyCollection<CraftingRecipeData> NewRecipes => _newRecipes;
        public IReadOnlyList<CraftingSkillResult> SkillResults => _skillResults;
        public int CraftingLevel => LevelInfo.Level;
        public LevelInfo LevelInfo { get; private set; }
        public CraftingItem CraftingItemModel { get; private set; }
        public RpgActor CraftingActor { get; private set; }
        #endregion

        #region MonoBehaviour Methods
        private void OnDisable()
        {
            if(InventoryManager.Instance != null)
                InventoryManager.Instance.OnItemAddedEvent -= HandleOnItemAdded;
        }
        #endregion

        #region ISaveable Implementation
        public IEnumerable<Type> SaveDataTypes => new[] { typeof(CraftingSaveData) };

        public bool TryLoadData(SaveDataBase saveData)
        {
            if (saveData is not CraftingSaveData inventorySaveData)
                return false;

            InitializeFromSaveData(inventorySaveData);
            return true;
        }

        public SaveDataBase SaveData()
        {
            CraftingSaveData saveData = new()
            {
                CraftingLevel = LevelInfo.Level,
                TotalExperience = LevelInfo.TotalExperience,
            };
            foreach (CraftingRecipeData recipe in _learnedRecipes)
                saveData.LearnedRecipes.Add(recipe.Item.Id.Id);

            foreach (CraftingRecipeData recipe in _newRecipes)
                saveData.NewRecipes.Add(recipe.Item.Id.Id);

            return saveData;
        }
        #endregion

        #region Public Methods
        public void InitializeAsNew()
        {
            Initialize();
            LearnBaseRecipes();
        }

        public void MarkNewRecipesAsLearned()
        {
            _learnedRecipes.AddRange(_newRecipes);
            _newRecipes.Clear();
        }

        public int ComputeRecipeExperience(CraftingRecipeData recipe)
        {
            //TODO: Balance xp formula
            //Formula: 10*RecipeLevel / (CraftLevel - RecipeLevel + 1)
            float levelDelta = CraftingLevel - recipe.RecipeLevel + 1;
            int amount = Mathf.Max(Mathf.FloorToInt(recipe.Experience / levelDelta), 1);

            if(TryProcCraftingSkill(CraftingSkillType.BonusXp, out int effectValue))
                amount += Mathf.FloorToInt(amount * effectValue / 100f);
            return amount;
        }

        public void GainExperience(int amount)
        {
            LevelInfo.GainExperience(amount);
        }

        public static IEnumerable<InventoryItem> CollectCompatibleItems(IngredientData ingredient)
        {
            if (ingredient.IsCategory)
                return InventoryManager.Instance.GetCategoryItemsCopy(ingredient.Category);
            else
                return InventoryManager.Instance.GetAllItemSlots(ingredient.Item.Id);
        }

        public static int GetOwnedIngredientQuantity(IngredientData ingredient)
        {
            return CollectCompatibleItems(ingredient).Sum(item => item.Quantity);
        }

        public IReadOnlyCollection<CraftingSkillData> GetActorCraftingSkills(ActorId actor)
        {
           return _craftingSkillsMap.GetActorSkills(actor);
        }

        public IReadOnlyCollection<CraftingSkillData> GetOrderedActorCraftingSkills(ActorId actor)
        {
            IReadOnlyCollection<CraftingSkillData> skills = GetActorCraftingSkills(actor);
            return skills.OrderByDescending(OrderSkills).ToList();

            int OrderSkills(CraftingSkillData skill)
            {
                return skill.AddedEffect != null && !skill.AddedEffect.HasCompatibleType(CraftingItemModel.Type) ? -1 : 1;
            }
        }

        public bool TryGetActiveSkill(CraftingSkillType skillType, out CraftingSkillData skill)
        {
            skill = null;
            if (CraftingActor == null)
                return false;

            IReadOnlyCollection<CraftingSkillData> activeSkills = GetActorCraftingSkills(CraftingActor.Id);
            skill = activeSkills.FirstOrDefault(s => s.SkillType == skillType);
            return skill != null;
        }

        public bool TryProcCraftingSkill(CraftingSkillType skillType, out int effectValue)
        {
            effectValue = 0;
            if (TryGetActiveSkill(skillType, out CraftingSkillData skill) && skill.Proc())
            {
                effectValue = skill.EffectValue;
                AddProccedSkill(skill, effectValue);
                return true;
            }
            return false;
        }

        public bool HasRequiredCraftingLevel(CraftingRecipeData recipe)
        {
            return recipe.RecipeLevel <= CraftingLevel;
        }

        public bool CanCraftRecipe(CraftingRecipeData recipe)
        {
            if (!HasRequiredCraftingLevel(recipe))
                return false;

            foreach (IngredientData ingredient in recipe.Ingredients)
            {
                if (ingredient.Quantity > GetOwnedIngredientQuantity(ingredient))
                    return false;
            }
            return true;
        }

        public void CreateCraftingModel(CraftingRecipeData recipe)
        {
            CraftingItemModel = new CraftingItem(recipe);
        }

        public void AssignIngredientToSlot(int slotIndex, RpgItem item)
        {
            EmptyIngredientSlot(slotIndex);
            CraftingItemModel.AssignIngredientToSlot(slotIndex, item);
            InventoryManager.Instance.RemoveItem(item, 1);
        }

        public void EmptyIngredientSlot(int slotIndex)
        {
            RpgItem item = CraftingItemModel.EmptyIngredientSlot(slotIndex);
            if(item != null)
                InventoryManager.Instance.AddBackItem(item, 1);
        }

        public void ChangeActor(RpgActor actor)
        {
            RemoveSkillEffects();
            CraftingActor = actor;
            AddSkillEffects();
        }

        public void ClearActor()
        {
            CraftingActor = null;
        }

        public void CancelCurrentCrafting()
        {
            for (int i = 0; i < CraftingItemModel.IngredientCount; i++)
            {
                if (CraftingItemModel.IsSlotEmpty(i))
                    continue;

                RpgItem item = CraftingItemModel.GetIngredientInSlot(i);
                InventoryManager.Instance.AddBackItem(item);
            }
            CraftingItemModel = null;
        }

        public bool HasNewRecipes()
        {
            return NewRecipes.Count > 0;
        }

        /// <summary>
        /// Craft the given recipe by auto-selecting the materials to use.
        /// Assumes that the player owns enough Materials
        /// </summary>
        /// <param name="recipe"></param>
        public void AutoCraftRecipe(CraftingRecipeData recipe)
        {
            CreateCraftingModel(recipe);
            for (int i = 0; i < CraftingItemModel.IngredientCount; i++)
            {
                IngredientData ingredient = CraftingItemModel.GetIngredientDataForSlot(i);
                InventoryItem inventoryItem = CollectCompatibleItems(ingredient).OrderByDescending(item => item.Quantity).First();
                AssignIngredientToSlot(i, inventoryItem.Item);
            }
            CraftItemModel();
        }

        /// <summary>
        /// Craft an item based on the model inside the CraftingItem.
        /// Assumes that the model is valid.
        /// </summary>
        /// <param name="itemModel">Item to create</param>
        public void CraftItemModel()
        {
            _skillResults.Clear();
            //Is there a way to NOT create an item if one already exists?
            RpgItem craftedItem = CraftingItemModel.MakeItem();

            int quantity = 1;
            if(CraftingActor != null)
            {
                if (TryProcCraftingSkill(CraftingSkillType.Quantity, out int effectValue))
                    quantity += effectValue;

                for (int i = 0; i < CraftingItemModel.IngredientCount; i++)
                {
                    if (TryProcCraftingSkill(CraftingSkillType.SaveMaterials, out _))
                        InventoryManager.Instance.AddBackItem(CraftingItemModel.GetIngredientInSlot(i));
                }
            }

            InventoryManager.Instance.AddItem(craftedItem, quantity);
            CraftingItemModel = null;
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            InventoryManager.Instance.OnItemAddedEvent -= HandleOnItemAdded;
            InventoryManager.Instance.OnItemAddedEvent += HandleOnItemAdded;
            _learnedRecipes.Clear();
            _newRecipes.Clear();
            _skillResults.Clear();
            LevelInfo = new LevelInfo(_maxLevel, ComputeExperienceForLevel);
            LevelInfo.OnLevelUpEvent += HandleOnLevelUp;
        }

        private void InitializeFromSaveData(CraftingSaveData saveData)
        {
            Initialize();
            LevelInfo.LoadData(saveData.CraftingLevel, saveData.TotalExperience);
            foreach(string itemId in saveData.LearnedRecipes)
                _learnedRecipes.Add(GetRecipe((ItemId)itemId));

            foreach (string itemId in saveData.NewRecipes)
                _newRecipes.Add(GetRecipe((ItemId)itemId));
        }

        /// <summary>
        /// Learn the Level 1 recipes and by-pass the new recipes popup.
        /// </summary>
        private void LearnBaseRecipes()
        {
            foreach (CraftingRecipeData recipe in _allRecipes)
            {
                if (recipe.RecipeLevel == 1)
                    _learnedRecipes.Add(recipe);
            }
        }

        private CraftingRecipeData GetRecipe(ItemId id)
        {
            return _allRecipes.FirstOrDefault(recipe => recipe.Item.Id == id);
        }

        private void LearnRecipe(CraftingRecipeData recipe)
        {
            if (HasLearnedRecipe(recipe))
                return;
            _newRecipes.Add(recipe);
        }

        private bool HasLearnedRecipe(CraftingRecipeData recipe)
        {
            return _learnedRecipes.Find(r =>  r == recipe) != null || _newRecipes.Find(r => r == recipe) != null;
        }

        private int ComputeExperienceForLevel(int level)
        {
            //TODO: Balance formula.
            //Experience formula: x^3
            return Mathf.FloorToInt(Mathf.Pow(level, 3));
        }

        private void HandleOnLevelUp()
        {
            foreach (CraftingRecipeData recipe in _allRecipes)
            {
                if (CraftingLevel >= recipe.RecipeLevel)
                    LearnRecipe(recipe);
            }
        }

        private void HandleOnItemAdded(InventoryItem item)
        {
            if (item == null)
                return;

            CraftingRecipeData recipe = GetRecipe(item.Item.Id);
            if(recipe != null)
                LearnRecipe(recipe);
        }
               

        private void AddSkillEffects()
        {
            if (TryGetActiveSkill(CraftingSkillType.AddEffect, out CraftingSkillData skill))
                CraftingItemModel.AddSkillEffect(new ItemEffect(skill.AddedEffect, skill.EffectValue));
        }

        private void RemoveSkillEffects()
        {
            if (TryGetActiveSkill(CraftingSkillType.AddEffect, out CraftingSkillData skill))
                CraftingItemModel.RemoveSkillEffect(new ItemEffect(skill.AddedEffect, skill.EffectValue));
        }

        private void AddProccedSkill(CraftingSkillData skill, int effectValue)
        {
            CraftingSkillResult existingResult = _skillResults.Find(result => result.Skill == skill);
            if (existingResult == null)
            {
                existingResult = new CraftingSkillResult(skill);
                _skillResults.Add(existingResult);
            }
            existingResult.AddResultValue(effectValue);
        }
        #endregion
    }
}

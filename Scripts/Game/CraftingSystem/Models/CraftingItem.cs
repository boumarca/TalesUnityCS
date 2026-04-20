using System.Collections.Generic;
using Framework.Extensions;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;

namespace Game.CraftingSystem.Models
{
    public class CraftingItem
    {
        #region Private Fields
        private List<IngredientSlot> _ingredientSlots = new();
        private List<ItemEffect> _inheritedEffects = new();
        private List<ItemEffect> _selectedEffects = new(RpgItem.MaxEffectCount);
        private List<ItemEffect> _effectsRaw = new();
        #endregion

        #region Public Properties
        public CraftingRecipeData Recipe { get; private set; }
        public ItemType Type => Recipe.Item.Type;
        public IReadOnlyCollection<ItemEffect> InheritedEffects => _inheritedEffects;
        public IReadOnlyCollection<ItemEffect> SelectedEffects => _selectedEffects;
        public int IngredientCount => _ingredientSlots.Count;        
        #endregion

        #region Constructors
        public CraftingItem(CraftingRecipeData recipe)
        {
            Recipe = recipe;
            InitializeIngredientSlots(recipe);
        }
        #endregion

        #region Public Methods
        public RpgItem MakeItem()
        {
            return new RpgItem(Recipe.Item, SelectedEffects);
        }

        public void AssignIngredientToSlot(int slotId, RpgItem item)
        {
            if (GetIngredientInSlot(slotId) == item)
                return;

            _ingredientSlots[slotId].AssignedItem = item;
            AddItemEffects(item);
        }

        public IngredientData GetIngredientDataForSlot(int slotId)
        {
            return _ingredientSlots[slotId].RequiredIngredient;
        }

        public RpgItem GetIngredientInSlot(int slotId)
        {
            return _ingredientSlots[slotId].AssignedItem;
        }

        public RpgItem EmptyIngredientSlot(int slotId)
        {
            if (IsSlotEmpty(slotId))
                return null;

            RpgItem item = GetIngredientInSlot(slotId);
            RemoveItemEffects(item);
            _ingredientSlots[slotId].AssignedItem = null;
            return item;
        }

        public bool IsSlotEmpty(int slotId)
        {
            return _ingredientSlots[slotId].IsEmpty();
        }

        public bool HasEmptySlot()
        {
            foreach (IngredientSlot slot in _ingredientSlots)
            {
                if (slot.IsEmpty())
                    return true;
            }
            return false;
        }

        public bool IsNewEffect(ItemEffect effect)
        {
            return _effectsRaw.None(e => e.IsSameEffect(effect));
        }

        public void AddSkillEffect(ItemEffect effect)
        {
            _effectsRaw.Add(effect);
            AddEffect(effect);
        }

        public void RemoveSkillEffect(ItemEffect effect)
        {
            _effectsRaw.Remove(effect);
            RemoveEffect(effect);
        }

        public void SelectEffect(ItemEffect effect)
        {
            if(!IsEffectSelected(effect) && _selectedEffects.Count < RpgItem.MaxEffectCount)
                _selectedEffects.Add(effect);
        }

        public void DeselectEffect(ItemEffect effect)
        {
            _selectedEffects.Remove(effect);
        }

        public bool IsEffectSelected(ItemEffect effect)
        {
            return _selectedEffects.Contains(effect);
        }

        public void DeselectAllEffects()
        {
            _selectedEffects.Clear();
        }

        public int GetAssignCount(RpgItem item)
        {
            int count = 0;
            foreach(IngredientSlot slot in _ingredientSlots)
            {
                if (slot.AssignedItem == item)
                    count++;
            }
            return count;
        }
        #endregion

        #region Private Methods
        private void InitializeIngredientSlots(CraftingRecipeData recipe)
        {
            foreach (IngredientData ingredientType in recipe.Ingredients)
            {
                for (int i = 0; i < ingredientType.Quantity; i++)
                    _ingredientSlots.Add(new IngredientSlot(ingredientType));
            }
        }

        private void AddItemEffects(RpgItem item)
        {
            foreach (ItemEffect effect in item.Effects)
            {
                _effectsRaw.Add(effect);
                AddEffect(effect.TransferEffect());
            }
        }

        private void AddEffect(ItemEffect effect)
        {
            if (!effect.IsCompatible(Recipe.Item.Type))
                return;

            if (!TryGetExistingEffectsOfType(effect.EffectType, out List<ItemEffect> existingEffects))
            {
                _inheritedEffects.Add(effect);
                return;
            }

            if(!TryFindMatchingLevelEffect(existingEffects, effect.Level, out ItemEffect sameLevelEffect) || sameLevelEffect.IsLevelMax())
            {
                _inheritedEffects.Add(effect);
                return;
            }

            _selectedEffects.Remove(sameLevelEffect);
            sameLevelEffect.LevelUp();

            if(existingEffects.Count > 1)
            {
                //remove and add the same effect in order to check for multiple level ups.
                _inheritedEffects.Remove(sameLevelEffect);
                AddEffect(sameLevelEffect);
            }
        }

        private void RemoveItemEffects(RpgItem item)
        {
            foreach (ItemEffect effect in item.Effects)
            {
                _effectsRaw.Remove(effect);
                RemoveEffect(effect);
            }
        }

        private void RemoveEffect(ItemEffect effect)
        {
            if (!TryGetExistingEffectsOfType(effect.EffectType, out List<ItemEffect> existingEffects))
                return;

            if (TryFindMatchingLevelEffect(existingEffects, effect.Level, out ItemEffect sameLevelEffect))
            {
                _inheritedEffects.Remove(sameLevelEffect);
                _selectedEffects.Remove(sameLevelEffect);
                return;
            }

            if (!TryFindHighestLevelEffect(existingEffects, effect.Level, out ItemEffect higherLevelEffect))
                return;

            higherLevelEffect.LevelDown();
            _selectedEffects.Remove(higherLevelEffect);
            //Add a copy of the level down effect and continue to try to remove the original effect.
            _inheritedEffects.Add(higherLevelEffect.TransferEffect());
            RemoveEffect(effect);
        }

        private bool TryGetExistingEffectsOfType(EffectType type, out List<ItemEffect> effectList)
        {
            effectList = _inheritedEffects.FindAll(e => e.EffectType == type);
            return effectList.Count > 0;
        }

        private static bool TryFindMatchingLevelEffect(List<ItemEffect> itemEffects, int level, out ItemEffect effect)
        {
            effect = itemEffects.Find(e => e.Level == level);
            return effect != null;
        }

        private static bool TryFindHighestLevelEffect(List<ItemEffect> itemEffects, int minimumLevel, out ItemEffect higherLevelEffect)
        {
            higherLevelEffect = null;
            int lowestLevel = int.MaxValue;
            foreach (ItemEffect e in itemEffects)
            {
                if (e.Level <= minimumLevel || e.Level >= lowestLevel)
                    continue;

                higherLevelEffect = e;
                lowestLevel = e.Level;
            }
            return higherLevelEffect != null;
        }
        #endregion
    }
}

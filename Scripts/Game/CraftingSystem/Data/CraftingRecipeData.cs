using System.Collections.Generic;
using Game.RpgSystem.Data;
using UnityEngine;

namespace Game.CraftingSystem.Data
{
    [CreateAssetMenu(fileName = "NewCraftingRecipeData", menuName = "Game Data/Crafting/CraftingRecipeData")]
    public class CraftingRecipeData : ScriptableObject
    {
        [field: SerializeField] public RpgItemData Item { get; private set; }
        [field: SerializeField] public int RecipeLevel { get; private set; }
        [SerializeField] private IngredientData[] _ingredients;

        public IReadOnlyCollection<IngredientData> Ingredients => _ingredients;
        public int Experience => RecipeLevel * 10; //TODO: Balance
    }
}

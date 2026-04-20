using Framework.Data;
using Game.CraftingSystem.Data;
using UnityEngine;

namespace Game.RpgSystem
{
    [CreateAssetMenu(fileName = "NewCraftingRecipeDatabase", menuName = "Databases/CraftingRecipeDatabase")]
    public class CraftingRecipeDatabase : Database<CraftingRecipeData>
    {
    }
}

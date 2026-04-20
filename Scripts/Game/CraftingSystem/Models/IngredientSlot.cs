using Game.CraftingSystem.Data;
using Game.RpgSystem.Models;

namespace Game.CraftingSystem.Models
{
    public class IngredientSlot
    {
        #region Public Properties
        public IngredientData RequiredIngredient { get; private set; }
        public RpgItem AssignedItem { get; set; }
        #endregion

        #region Constructor
        public IngredientSlot(IngredientData requiredIngredient)
        {
            RequiredIngredient = requiredIngredient;
        }
        #endregion

        #region Public Methods
        public bool IsEmpty()
        {
            return AssignedItem == null;
        }
        #endregion
    }
}

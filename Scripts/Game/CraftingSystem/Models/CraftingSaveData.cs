using System.Collections.Generic;
using Game.SaveSystem;

namespace Game.CraftingSystem.Models
{
    public class CraftingSaveData : SaveDataBase
    {
        #region Public Properties
        public int CraftingLevel { get; set; }
        public int TotalExperience { get; set; }
        public ICollection<string> LearnedRecipes { get; set; } = new List<string>();
        public ICollection<string> NewRecipes { get; set; } = new List<string>();
        #endregion
    }
}

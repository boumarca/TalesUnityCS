using System.Globalization;
using Game.CraftingSystem;
using TMPro;
using UnityEngine;

namespace Jrpg.Menus.Crafting
{
    public class CraftingLevelWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private TextMeshProUGUI _craftingLevelText;
        [SerializeField] private ExperienceBar _expBar;
        #endregion

        #region Public Methods
        public void Refresh()
        {
            _craftingLevelText.text = CraftingManager.Instance.CraftingLevel.ToString(CultureInfo.InvariantCulture);
            _expBar.FillExpBar(CraftingManager.Instance.LevelInfo);
        }
        #endregion
    }
}

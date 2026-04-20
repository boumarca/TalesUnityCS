using System.Globalization;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;

namespace Jrpg.Menus
{
    public class ExperienceBar : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private LabelBar _bar;
        #endregion

        #region Public Methods
        public void FillExpBar(LevelInfo levelInfo)
        {
            int remaining = levelInfo.RemainingExperienceToNextLevel();
            float progress = levelInfo.GetProgressPercentToNextLevel();
            _bar.SetBarValue(remaining.ToString(CultureInfo.InvariantCulture), progress);
        }
        #endregion
    }
}

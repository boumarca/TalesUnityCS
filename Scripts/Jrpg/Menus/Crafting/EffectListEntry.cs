using Game.RpgSystem.Models;
using Game.UI;
using TMPro;
using UnityEngine;

namespace Jrpg.Menus.Crafting
{
    public class EffectListEntry : ListEntry
    {
        #region Serialized Fields
        [SerializeField] private TextMeshProUGUI _effectNameText;
        [SerializeField] private Checkbox _checkbox;
        [SerializeField] private Color _leveledUpColor;
        #endregion

        #region Public Properties
        public bool IsChecked => _checkbox.IsChecked;
        #endregion

        #region ListEntry Implementation
        protected override void OnInitialize()
        {
            InitializeEffect(Data as ItemEffect);
        }
        #endregion

        #region Public Methods
        public void SetChecked(bool isChecked)
        {
            _checkbox.SetChecked(isChecked);
        }

        public void SetLeveledUp(bool isLeveledUp)
        {
            _effectNameText.color = isLeveledUp ? _leveledUpColor : Color.white;
        }
        #endregion

        #region Private Methods
        private void InitializeEffect(ItemEffect effect)
        {
            _effectNameText.text = effect.DisplayName;
        }
        #endregion
    }
}

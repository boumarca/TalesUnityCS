using System.Globalization;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using SerializedTuples;
using SerializedTuples.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class EffectInfoView : InfoView<ItemEffect>
    {
        #region Serialized Fields
        [SerializeField] private TextMeshProUGUI _effectNameText;
        [SerializeField] private TextMeshProUGUI _effectDescriptionText;
        [SerializeField] private TextMeshProUGUI _effectLevelText;
        [SerializedTupleLabels("ItemType", "Image")]
        [SerializeField] private SerializedTuple<ItemType, Image>[] _itemIcons;
        #endregion

        #region InfoView Implementation
        protected override void DisplayInfos()
        {
            _effectNameText.text = SelectedInfo.DisplayName;
            _effectDescriptionText.text = SelectedInfo.DisplayDescription;
            _effectLevelText.text = SelectedInfo.MaxLevel.ToString(CultureInfo.InvariantCulture);
            DisplayCompatibleItemTypes();
        }
        #endregion

        #region Private Methods
        private void DisplayCompatibleItemTypes()
        {
            foreach (SerializedTuple<ItemType, Image> icon in _itemIcons)
                icon.v2.color = SelectedInfo.IsCompatible(icon.v1) ? Color.white : Color.gray3;
        }
        #endregion
    }
}

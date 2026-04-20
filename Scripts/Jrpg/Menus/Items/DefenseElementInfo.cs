using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jrpg.Menus.Items
{
    public class DefenseElementInfo : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _value;
        #endregion

        #region Public Methods
        public void SetInfos(Sprite icon, int value)
        {
            _icon.sprite = icon;
            _value.text = $"{(value > 0 ? "+" : string.Empty)}{value}%";
        }
        #endregion
    }
}

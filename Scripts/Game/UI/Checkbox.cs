using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class Checkbox : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image _checkbox;
        [SerializeField] private Sprite _emptyBoxSprite;
        [SerializeField] private Sprite _checkedBoxSprite;
        #endregion

        #region Public Properties
        public bool IsChecked { get; private set; }
        #endregion

        #region Public Methods
        public void SetChecked(bool isChecked)
        {
            IsChecked = isChecked;
            _checkbox.sprite = IsChecked ? _checkedBoxSprite : _emptyBoxSprite;
        }
        #endregion
    }
}

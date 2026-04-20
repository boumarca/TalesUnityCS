using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Game.UI
{
    public class Tab : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image _graphic;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private LocalizedString _tabName;

        [SerializeField] private UnityEvent _onTabSelected;
        #endregion

        #region Private Fields
        private Color _baseColor;
        #endregion

        #region Public Properties
        public LocalizedString TabName => _tabName;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            _baseColor = _graphic.color;
        }
        #endregion

        #region Public Methods
        public void SelectTab()
        {
            _graphic.color = _selectedColor;
            _onTabSelected?.Invoke();
        }

        public void DeselectTab()
        {
            _graphic.color = _baseColor;
        }
        #endregion
    }
}

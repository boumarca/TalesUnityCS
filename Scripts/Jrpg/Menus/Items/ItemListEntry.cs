using Game.RpgSystem.Models;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Items
{
    public class ItemListEntry : ListEntry
    {
        #region Serialized Fields
        [SerializeField] private LocalizeStringEvent _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemQuantityText;
        [SerializeField] private Image _itemIcon;
        #endregion

        #region Private Fields
        private InventoryItem _item;
        #endregion

        #region MenuListEntry Implementation
        protected override void OnInitialize()
        {
            _item = Data as InventoryItem;
            _itemNameText.StringReference = _item.Item.Name;
            _itemIcon.sprite = _item.Item.Icon;
            RefreshQuantity();
#if UNITY_EDITOR
            name = _item.Item.Name.GetLocalizedString();
#endif
        }
        #endregion

        #region Public Methods
        public void RefreshQuantity()
        {
            _itemQuantityText.text = $": {_item.Quantity}";
        }
        #endregion
    }
}

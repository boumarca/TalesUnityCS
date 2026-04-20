using System;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Equip
{
    public class EquipSlotEntry : MonoBehaviour, ISelectHandler
    {
        #region Serialized Field
        [SerializeField] private Image _itemIcon;
        [SerializeField] private LocalizeStringEvent _itemNameText;
        [SerializeField] private EquipSlot _slotType;
        [SerializeField] private ItemType _slotItemType;
        #endregion

        #region Public Properties
        public EquipSlot SlotType => _slotType;
        public ItemType SlotItemType => _slotItemType;
        public RpgItem EquippedItem { get; private set; }
        #endregion

        #region Events
        public event Action<EquipSlotEntry> OnSlotSelectedEvent = delegate { };
        #endregion

        #region ISelectHandler Implementation
        public void OnSelect(BaseEventData eventData)
        {
            OnSlotSelectedEvent(this);
        }
        #endregion

        #region Public Methods
        public void ChangeItem(RpgItem equippedItem)
        {
            EquippedItem = equippedItem;
            if (EquippedItem != null)
                DisplayEquipment();
            else
                DisplayEmptySlot();
        }
        #endregion

        #region Private Methods
        private void DisplayEquipment()
        {
            _itemIcon.gameObject.SetActive(true);
            _itemIcon.sprite = EquippedItem.Icon;
            _itemNameText.gameObject.SetActive(true);
            _itemNameText.StringReference = EquippedItem.Name;
        }

        private void DisplayEmptySlot()
        {
            _itemIcon.gameObject.SetActive(false);
            _itemNameText.gameObject.SetActive(false);
        }
        #endregion
    }
}

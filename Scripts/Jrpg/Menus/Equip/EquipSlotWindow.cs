using System;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jrpg.Menus.Equip
{
    public class EquipSlotWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ActorHeader _actorHeader;
        [SerializeField] private EquipSlotEntry _defaultSelection;
        [SerializeField] private InputtableWindow _inputWindow;
        [SerializeField] private EquipSlotEntry[] _slots;
        #endregion

        #region Private Fields
        private EquipSlotEntry _selectedSlot;
        #endregion

        #region Public Properties
        public EquipSlot SelectedSlotType => _selectedSlot.SlotType;
        public RpgItem SelectedSlotItem => _selectedSlot.EquippedItem;
        public RpgActor Actor { get; private set; }
        #endregion

        #region Events
        public event Action<EquipSlotEntry> OnSelectedSlotChangedEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _inputWindow.OnActivatedEvent += HandleOnActivated;
            foreach (EquipSlotEntry slotEntry in _slots)
                slotEntry.OnSlotSelectedEvent += HandleOnSlotSelected;
        }

        private void OnDisable()
        {
            _inputWindow.OnActivatedEvent -= HandleOnActivated;
            foreach (EquipSlotEntry slotEntry in _slots)
                slotEntry.OnSlotSelectedEvent -= HandleOnSlotSelected;
        }
        #endregion

        #region Public Methods
        public void ChangeActor(RpgActor actor)
        {
            Actor = actor;
            Refresh();
        }

        public void ChangeSelectedSlotItem(RpgItem item)
        {
            _selectedSlot.ChangeItem(item);
        }

        public void Refresh()
        {
            _actorHeader.Refresh(Actor);
            foreach (EquipSlotEntry slotEntry in _slots)
                slotEntry.ChangeItem(Actor.GetEquippedItem(slotEntry.SlotType));
        }
        #endregion

        #region Private Methods
        private void HandleOnActivated()
        {
            EquipSlotEntry slotToSelect = _selectedSlot != null ? _selectedSlot : _defaultSelection;
            EventSystem.current.SetSelectedGameObject(slotToSelect.gameObject);
        }

        private void HandleOnSlotSelected(EquipSlotEntry equipSlot)
        {
            if (_selectedSlot == equipSlot)
                return;

            _selectedSlot = equipSlot;
            OnSelectedSlotChangedEvent(_selectedSlot);
        }
        #endregion
    }
}

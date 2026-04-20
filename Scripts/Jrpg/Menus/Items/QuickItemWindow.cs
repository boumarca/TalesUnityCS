using System;
using System.Collections.Generic;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jrpg.Menus.Items
{
    public class QuickItemWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private QuickItemEntry _entryPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private Game.UI.Cursor _cursor;
        #endregion

        #region Private Fields
        private readonly List<QuickItemEntry> _entries = new();
        private RpgItem _currentItem;
        private QuickItemEntry _currentSelection;
        #endregion

        #region Events
        public event Action<RpgActor, RpgItem> OnActorConfirmedEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            UIManager.Instance.OnSelectionChangedEvent += HandleOnSelectionChanged;
        }

        private void OnDisable()
        {
            if (UIManager.Instance != null)
                UIManager.Instance.OnSelectionChangedEvent -= HandleOnSelectionChanged;
        }
        #endregion

        #region Public Methods
        public void DisplayWindow(RpgItem item)
        {
            Clear();
            _currentItem = item;
            CreatePartyMembersEntries();
            EventSystem.current.SetSelectedGameObject(_entries[0].gameObject);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            _cursor.ResetPosition();
        }
        #endregion

        #region Private Methods
        private void CreatePartyMembersEntries()
        {
            foreach(RpgActor actor in PartyManager.Instance.CurrentParty.Members)
            {
                QuickItemEntry entry = Instantiate(_entryPrefab, _content);
                entry.Initialize(actor, _currentItem);
                entry.Refresh();
                entry.OnActorConfirmedEvent += HandleOnActorConfirmed;
                _entries.Add(entry);
            }
            _gridLayout.constraintCount = _entries.Count > 1 ? 2 : 1;
        }

        private void Clear()
        {
            foreach (QuickItemEntry entry in _entries)
                Destroy(entry.gameObject);

            _entries.Clear();
        }

        //TODO: Handle selection of all actors
        private void HandleOnActorConfirmed(RpgActor actor)
        {
            OnActorConfirmedEvent(actor, _currentItem);
            _currentSelection.Refresh();
        }

        private void HandleOnSelectionChanged(object sender, EventArgs args)
        {
            GameObject selection = EventSystem.current.currentSelectedGameObject;
            if (selection == null || !selection.TryGetComponent(out QuickItemEntry listEntry))
                return;

            if(_currentSelection != null)
                _currentSelection.Deselect();

            _currentSelection = listEntry;
            _currentSelection.Select();
        }
        #endregion
    }
}

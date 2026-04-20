using System;
using System.Collections.Generic;
using Framework.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class ListView : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ListEntry _listEntryPrefab;
        [SerializeField] private InputtableWindow _inputWindow;
        [SerializeField] private GridLayoutGroup _listLayout;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private AutoScrollRectContent _autoScroll;
        [SerializeField] private GameObject _upArrow;
        [SerializeField] private GameObject _downArrow;
        [SerializeField] private GameObject _emptySelection;
        #endregion

        #region Private Fields
        private readonly List<ListEntry> _listEntries = new();
        private ListEntry _selectedListEntry;
        private int _currentIndex;
        private ListEntry _lastValidSelection;
        #endregion

        #region Public Properties
        public ListEntry SelectedListEntry => _selectedListEntry;
        public IReadOnlyList<ListEntry> Entries => _listEntries;
        public int EntryCount => _listEntries.Count;
        public GameObject Last => EntryCount > 0 ? Entries[^1].gameObject : _emptySelection;
        #endregion

        #region Events
        public event Action<ListEntry> OnSelectedEntryChangedEvent = delegate { };
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
        public void PopulateList(IEnumerable<object> list, Action<object> onConfirm = null, bool keepIndex = false)
        {
            Clear();
            foreach (object data in list)
                CreateListEntry(data, onConfirm);

            CheckEmptySelectionVisibility();

            if (_inputWindow.IsActive)
                SelectIndex(keepIndex ? _currentIndex : 0);
            else
                _inputWindow.DisableContent();
        }

        public void AddEntry(ListEntry entry)
        {
            _listEntries.Add(entry);
            CheckEmptySelectionVisibility();
        }

        public void InsertEntry(int index, ListEntry entry)
        {
            _listEntries.Insert(index, entry);
            CheckEmptySelectionVisibility();
        }

        public void RemoveSelectedEntry()
        {
            if (_selectedListEntry == null)
                return;
                        
            _listEntries.RemoveAt(_currentIndex);
            CheckEmptySelectionVisibility();
            Destroy(_selectedListEntry.gameObject);
            SelectIndex(_currentIndex);
        }

        public void SelectCurrent()
        {
            SelectIndex(_currentIndex);
        }

        public void SelectIndex(int newIndex)
        {
            _currentIndex = Mathf.Clamp(newIndex, 0, _listEntries.Count - 1);
            GameObject nextEntry = _listEntries.Count > 0 ? _listEntries[_currentIndex].gameObject : _emptySelection;
            EventSystem.current.SetSelectedGameObject(nextEntry);
        }

        /// <summary>
        /// Select the next entry in the list that satisfies the condition of the predicate.
        /// </summary>
        /// <param name="predicate">Condition to satisfy</param>
        /// <param name="fallback">GameObject to select if no entry in the list matches</param>
        public void SelectNext(Predicate<ListEntry> predicate, GameObject fallback)
        {            
            int currentIndex = Entries.IndexOf(SelectedListEntry);
            for (int i = 1; i < EntryCount; i++)
            {
                int newIndex = (currentIndex + i) % EntryCount;
                ListEntry entry = Entries[newIndex];

                if (predicate(entry))
                {
                    EventSystem.current.SetSelectedGameObject(entry.gameObject);
                    return;
                }
            }

            EventSystem.current.SetSelectedGameObject(fallback);
        }

        public void ChangePage(float inputDirection)
        {
            //Performed should not return 0;
            ChangePage(inputDirection > 0 ? 1 : -1);
        }

        public void ChangePage(int direction)
        {
            int delta = GetEntriesPerPageCount();
            _autoScroll.ScrollPage(direction);
            SelectIndex(_currentIndex + delta * direction);
        }

        public void SelectLastValidEntry()
        {
            EventSystem.current.SetSelectedGameObject(_lastValidSelection.gameObject);
        }

        public void ClearSelection()
        {
            UpdateSelection(null);
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnScrollValueChanged(Vector2 scroll)
        {
            UpdateScrollArrows(scroll);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnListEntryCreated(ListEntry entry) { }
        #endregion

        #region Private Methods
        private void CreateListEntry(object data, Action<object> onConfirm)
        {
            ListEntry listEntry = Instantiate(_listEntryPrefab, _scrollRect.content);
            listEntry.Initialize(data);
            listEntry.OnConfirmItemEvent += onConfirm;
            _listEntries.Add(listEntry);
            OnListEntryCreated(listEntry);
        }

        private void CheckEmptySelectionVisibility()
        {
            _emptySelection.SetActive(_listEntries.Count == 0);
        }

        private void HandleOnSelectionChanged(object sender, EventArgs args)
        {
            if (!_inputWindow.IsActive)
                return;

            GameObject selection = EventSystem.current.currentSelectedGameObject;
            if (selection == null || !selection.TryGetComponent(out ListEntry listEntry))
            {
                UpdateSelection(null);
                return;
            }
            UpdateSelection(listEntry);
        }

        private void UpdateSelection(ListEntry listEntry)
        {
            _selectedListEntry = listEntry;

            if(_selectedListEntry != null)
                _lastValidSelection = _selectedListEntry;

            _currentIndex = _listEntries.IndexOf(_selectedListEntry);
            OnSelectedEntryChangedEvent(_selectedListEntry);
        }

        private void UpdateScrollArrows(Vector2 scroll)
        {
            if (_upArrow == null || _downArrow == null)
                return;

            bool canScroll = _scrollRect.IsScrollable();
            _upArrow.SetActive(canScroll && scroll.y < 1);
            _downArrow.SetActive(canScroll && scroll.y > 0);
        }

        private void Clear()
        {
            foreach (ListEntry entry in _listEntries)
                Destroy(entry.gameObject);

            _listEntries.Clear();
            _selectedListEntry = null;
            _lastValidSelection = null;
        }

        private int GetEntriesPerPageCount()
        {
            int rows = Mathf.FloorToInt(_scrollRect.viewport.rect.height / _listLayout.cellSize.y);
            return rows * _listLayout.constraintCount;
        }
        #endregion
    }
}

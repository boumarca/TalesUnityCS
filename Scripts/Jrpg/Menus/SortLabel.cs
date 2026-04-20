using System;
using System.Collections.Generic;
using Framework.Extensions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Jrpg.Menus
{
    public class SortLabel : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private LocalizeStringEvent _sortLabel;
        #endregion

        #region Private Fields
        private int _currentSortIndex;
        private SortingMode _currentSortMode;
        private IReadOnlyList<SortingMode> _sortOptions;
        #endregion

        #region Public Properties
        public SortingMode SortMode => _currentSortMode;
        #endregion

        #region Public Methods
        public void DisplaySortLabel(bool display)
        {
            _sortLabel.gameObject.SetActive(display);
        }

        public void ChangeSortMode()
        {
            ChangeSortIndex((_currentSortIndex + 1) % _sortOptions.Count);
        }

        public void ChangeSortOptions(IReadOnlyList<SortingMode> sortOptions)
        {
            _sortOptions = sortOptions;
            AdjustSortIndex();
        }
        #endregion

        #region Private Methods
        private void ChangeSortIndex(int newSortIndex)
        {
            _currentSortIndex = newSortIndex;
            _currentSortMode = _sortOptions[_currentSortIndex];
            RefreshSortLabel(_currentSortMode);
        }

        private void AdjustSortIndex()
        {
            int correctedIndex = _sortOptions.IndexOf(_currentSortMode);
            if (correctedIndex < 0)
                correctedIndex = Mathf.Min(_currentSortIndex, _sortOptions.Count - 1);

            ChangeSortIndex(correctedIndex);
        }

        private void RefreshSortLabel(SortingMode currentSortMode)
        {
            _sortLabel.StringReference = new LocalizedString("UI Table", $"$item_menu.sort_mode_{(int)currentSortMode}");
        }
        #endregion
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Game.UI
{
    public class TabList : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private List<Tab> _tabs;
        [SerializeField] private LocalizeStringEvent _tabHeaderText;
        #endregion

        #region Private Fields
        private int _selectedTabIndex;
        #endregion

        #region Properties
        private Tab SelectedTab => _tabs[_selectedTabIndex];
        public int Count => _tabs.Count;
        #endregion

        #region Public Methods
        public void AddTab(Tab tab)
        {
            _tabs.Add(tab);
            tab.transform.SetParent(transform, false);
        }

        public void SelectFirstTab()
        {
            SelectTab(0);
        }

        public void ChangeTab(int direction)
        {
            DeselectTab();
            int newIndex = (_selectedTabIndex + direction + _tabs.Count) % _tabs.Count;
            SelectTab(newIndex);
        }
        #endregion

        #region Private Methods
        private void SelectTab(int tabIndex)
        {
            _selectedTabIndex = tabIndex;
            SelectedTab.SelectTab();
            RefreshHeader();
        }

        private void DeselectTab()
        {
            SelectedTab.DeselectTab();
        }

        private void RefreshHeader()
        {
            if (_tabHeaderText == null)
                return;

            _tabHeaderText.gameObject.SetActive(true);
            _tabHeaderText.StringReference = SelectedTab.TabName;
        }
        #endregion
    }
}

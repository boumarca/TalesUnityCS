using UnityEngine;

namespace Jrpg.Menus
{
    public class InfoView<T> : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private int _defaultPage;
        [SerializeField] private GameObject[] _infosPages;
        #endregion

        #region Protected Properties
        protected T SelectedInfo { get; private set; }
        protected int CurrentPage { get; private set; }
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            CurrentPage = _defaultPage;
        }
        #endregion

        #region Public Methods
        public void Refresh(T item)
        {
            SelectedInfo = item;
            if (item == null)
            {
                HideInfos();
                return;
            }
            DisplayInfos();
            ShowValidPage();
        }

        public void ChangePage()
        {
            HideCurrentPage();
            CurrentPage = (CurrentPage + 1) % _infosPages.Length;
            ShowValidPage();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        #endregion

        #region Protected Methods
        protected virtual void DisplayInfos() { }
        protected virtual void ClearInfos() { }
        protected virtual bool IsPageValid() { return true; }
        #endregion

        #region Private Methods
        private void HideInfos()
        {
            ClearInfos();
            HideCurrentPage();
        }

        private void ShowValidPage()
        {
            if (IsPageValid())
            {
                HideCurrentPage();
                CurrentPage = 0;
            }
            _infosPages[CurrentPage].SetActive(true);
        }

        private void HideCurrentPage()
        {
            _infosPages[CurrentPage].SetActive(false);
        }
        #endregion
    }
}

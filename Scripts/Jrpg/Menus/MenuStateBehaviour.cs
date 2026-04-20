using System;
using System.Collections;
using System.Threading.Tasks;
using Framework.Inputs;
using Game.StateStack;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace Jrpg.Menus
{
    public class MenuStateBehaviour : GameStateBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private GameObject _defaultSelection;
        [SerializeField] private InputtableWindow _defaultWindow;
        [SerializeField] private Game.UI.Cursor _menuCursor;
        #endregion

        #region Private Fields
        private InputtableWindow _currentWindow;
        private GameObject _lastSelection;
        #endregion

        #region Protected Properties
        protected GameObject DefaultSelection => _defaultSelection;
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            UIManager.Instance.ShowMenu(transform);
            ChangeWindowFocus(_defaultWindow);
            EventSystem.current.SetSelectedGameObject(_defaultSelection);
            return Task.CompletedTask;
        }

        public override void OnExitState()
        {
            //TODO: Keep menu in cache once loaded.
            UIManager.Instance.HideMenuElements();
            ChangeWindowFocus(null);
            Destroy(gameObject);
        }

        public override void OnSuspendState()
        {
            _lastSelection = EventSystem.current.currentSelectedGameObject;
            _currentWindow.Deactivate();
            gameObject.SetActive(false);
        }

        public override void OnResumeState()
        {
            EventSystem.current.SetSelectedGameObject(_lastSelection);
            gameObject.SetActive(true);
            UIManager.Instance.ShowMenuElements();
            _currentWindow.Activate();
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnCancelPerformed()
        {
            CloseMenu();
        }
        #endregion

        #region Protected Methods
        protected void EnableConfirmInput()
        {
            InputManager inputManager = InputManager.Instance; 
            inputManager.EnableInput(inputManager.InputActions.Menus.Confirm);
        }

        protected void DisableConfirmInput()
        {
            InputManager inputManager = InputManager.Instance;
            inputManager.DisableInput(inputManager.InputActions.Menus.Confirm);
        }

        protected void RefreshInput(InputAction input, bool condition)
        {
            InputManager inputManager = InputManager.Instance;
            if (condition)
                inputManager.EnableInput(input);
            else
                inputManager.DisableInput(input);
        }

        protected void ChangeWindowFocus(InputtableWindow newWindow)
        {
            if(_currentWindow != null)
                _currentWindow.Deactivate();

            _currentWindow = newWindow;
            if(_currentWindow != null)
                _currentWindow.Activate();
        }

        protected void DisplayConfirmationPopup(LocalizedString bodyMessage, Action onConfirm, Action onCancel = null, bool selectConfirm = false)
        {
            _menuCursor.enabled = false;
            _currentWindow.Deactivate();
            GameObject previousSelection = EventSystem.current.currentSelectedGameObject;
            _menuCursor.gameObject.SetActive(true);
            UIManager.Instance.DisplayGenericConfirmationPopup(bodyMessage, OnConfirm, OnCancel, selectConfirm);
            return;

            void OnConfirm()
            {
                onConfirm?.Invoke();
                _menuCursor.enabled = true;
                _currentWindow.Activate();
            }

            void OnCancel()
            {
                onCancel?.Invoke();
                _menuCursor.enabled = true;
                _currentWindow.Activate();
                EventSystem.current.SetSelectedGameObject(previousSelection);
            }
        }

        protected void CloseMenu()
        {
            StartCoroutine(CloseMenuCoroutine());
        }
        #endregion

        #region Private Methods
        private IEnumerator CloseMenuCoroutine()
        {
            yield return null;
            StateStackManager.Instance.ReturnToPreviousState();
        }
        #endregion
    }
}

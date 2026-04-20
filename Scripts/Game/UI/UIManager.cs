using System;
using Framework.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace Game.UI
{
    public class UIManager : SceneSingleton<UIManager>
    {
        #region Serialized Fields
        [Header("GameObject references")]
        [SerializeField] private GameObject _darkOverlay;
        [SerializeField] private Textbox _textbox;
        [SerializeField] private GenericPopup _genericPopup;
        [SerializeField] private InputFooter _inputFooter;

        [Header("Localization strings")]
        [SerializeField] private LocalizedString _yesChoiceKey;
        [SerializeField] private LocalizedString _noChoiceKey;
        #endregion

        #region Private Fields
        private GameObject _currentSelection;
        #endregion

        #region Events
        public event EventHandler<EventArgs> OnTextboxClosedEvent = delegate { };
        public event EventHandler<EventArgs> OnSelectionChangedEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            GameObject selection = EventSystem.current.currentSelectedGameObject;
            if (_currentSelection == selection)
                return;

            _currentSelection = selection;
            OnSelectionChangedEvent(this, EventArgs.Empty);
        }
        #endregion

        #region Public Methods
        public void DisplayTextbox(string message, string speakerName)
        {
            _textbox.ShowMessage(message);
            if(!string.IsNullOrEmpty(speakerName))
                _textbox.ShowSpeakerName(speakerName);
        }

        public void HideTextbox()
        {
            _textbox.Close();
            OnTextboxClosedEvent(this, EventArgs.Empty);
        }

        public void ShowMenu(Transform menuTransform)
        {
            ShowMenuElements();
            menuTransform.SetParent(transform, false);
            menuTransform.SetSiblingIndex(1);
        }

        //TODO: Find a better way to handle show and hide overlay and input footer.
        public void ShowMenuElements()
        {
            _darkOverlay.SetActive(true);
            _inputFooter.gameObject.SetActive(true);
            _inputFooter.Clear();
        }

        public void HideMenuElements()
        {
            _darkOverlay.SetActive(false);
            _inputFooter.gameObject.SetActive(false);
        }

        public void ChangeInputLabel(InputAction action, LocalizedString newLabel)
        {
            _inputFooter.ChangeInputLabel(action, newLabel);
        }

        public void DisplayGenericPopup(LocalizedString bodyText)
        {
            _genericPopup.Initialize(bodyText);
            _genericPopup.transform.SetAsLastSibling();
        }

        public void DisplayGenericPopupWithChoices(LocalizedString bodyText, LocalizedString confirmText, Action onConfirm, LocalizedString cancelText, Action onCancel, bool selectConfirm = false)
        {
            _genericPopup.InitializeWithChoices(bodyText, confirmText, onConfirm, cancelText, onCancel, selectConfirm);
            _genericPopup.transform.SetAsLastSibling();
        }

        public void DisplayGenericConfirmationPopup(LocalizedString bodyText, Action onConfirm, Action onCancel, bool selectConfirm = false)
        {
            DisplayGenericPopupWithChoices(bodyText, _yesChoiceKey, onConfirm, _noChoiceKey, onCancel, selectConfirm);
        }
        #endregion
    }
}

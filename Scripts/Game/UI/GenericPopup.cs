using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Game.UI
{
    public class GenericPopup : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private InputtableWindow _inputtableWindow;
        [SerializeField] private LocalizeStringEvent  _textBody;
        [SerializeField] private LocalizeStringEvent _confirmText;
        [SerializeField] private LocalizeStringEvent _cancelText;
        [SerializeField] private GameObject _confirmChoiceObject;
        [SerializeField] private GameObject _cancelChoiceObject;
        [SerializeField] private Cursor _cursor;
        #endregion

        #region Private Fields
        private Action _onConfirm;
        private Action _onCancel;
        #endregion

        #region Public Methods
        public void Initialize(LocalizedString bodyText)
        {
            Clear();
            _textBody.StringReference = bodyText;
            Show();
        }

        public void InitializeWithChoices(LocalizedString bodyText, LocalizedString confirmText, Action onConfirm, LocalizedString cancelText, Action onCancel, bool selectConfirm = false)
        {
            Clear();
            _textBody.StringReference = bodyText;
            _confirmText.StringReference = confirmText;
            _cancelText.StringReference = cancelText;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            _confirmChoiceObject.SetActive(true);
            _cancelChoiceObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(selectConfirm ? _confirmChoiceObject : _cancelChoiceObject);
            Show();
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnConfirmButtonClicked()
        {
            Hide();
            _onConfirm?.Invoke();
        }

        public void UIOnCancelButtonClicked()
        {
            Hide();
            _onCancel?.Invoke();
        }
        #endregion

        #region Private Methods
        private void Show()
        {
            gameObject.SetActive(true);
            _inputtableWindow.Activate();
        }

        private void Hide()
        {
            _inputtableWindow.Deactivate();
            gameObject.SetActive(false);
            _cursor.ResetPosition();
        }

        private void Clear()
        {
            _textBody.StringReference.Clear();
            _confirmText.StringReference.Clear();
            _cancelText.StringReference.Clear();
            _onConfirm = null;
            _onCancel = null;
            _confirmChoiceObject.SetActive(false);
            _cancelChoiceObject.SetActive(false);
        }
        #endregion
    }
}

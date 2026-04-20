using System;
using System.Collections.Generic;
using Framework.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Game.UI
{
    public class InputHint : MonoBehaviour
    {
        #region Statics
        private static readonly List<InputBinding> s_bindingBuffer = new();
        #endregion

        #region Serialized Fields
        [SerializeField] private Image[] _inputIcons;
        [SerializeField] private LocalizeStringEvent _actionText;
        [SerializeField] private RectTransform _textTransform;
        #endregion

        #region Private Fields
        private InputAction _action;
        #endregion

        #region Public Properties
        public InputAction Action
        {
            get => _action;
            set
            {
                _action = value;
                RefreshIcon();
                SetInputText(GetLocalizedInput());
            }
        }
        #endregion

        #region MonoBehaviour Methods
        public void OnDisable()
        {
            _actionText.StringReference.Clear();
            _textTransform.sizeDelta = new Vector2(0, _textTransform.sizeDelta.y);
        }
        #endregion

        #region Public Methods
        public void RefreshIcon()
        {
            FindBindings();
            if(s_bindingBuffer.Count == 0)
            {
                Debug.LogWarning($"No binding found for action {Action} with control scheme {InputManager.Instance.CurrentControlScheme}");
                Hide();
                return;
            }
           
            SetInputIcons();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetInputText(LocalizedString text)
        {
            _actionText.StringReference = text;
        }
        #endregion

        #region Private Methods
        private int FindBindings()
        {
            s_bindingBuffer.Clear();
            string currentControlScheme = InputManager.Instance.CurrentControlScheme;
            foreach (InputBinding binding in Action.bindings)
            {
                if (binding.groups.Contains(currentControlScheme, StringComparison.Ordinal))
                    s_bindingBuffer.Add(binding);
            }
            return s_bindingBuffer.Count;
        }

        private void SetInputIcons()
        {
            IReadOnlyDictionary<string, Sprite> iconMap = InputManager.Instance.CurrentSchemeIconMapping;
            int bindingCount = s_bindingBuffer.Count;
            for (int i = 0; i < _inputIcons.Length; i++)
            {
                Image iconImage = _inputIcons[i];
                if (i >= bindingCount)
                {
                    iconImage.gameObject.SetActive(false);
                    continue;
                }
                iconImage.gameObject.SetActive(true);
                iconMap.TryGetValue(s_bindingBuffer[i].path, out Sprite icon);
                iconImage.sprite = icon;
                iconImage.SetNativeSize();
            }
        }

        private LocalizedString GetLocalizedInput()
        {
            return new LocalizedString("UI Table", $"$input.{_action.name.ToLowerInvariant()}");
        }
        #endregion
    }
}

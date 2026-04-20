using System;
using System.Collections.Generic;
using Framework.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Pool;

namespace Game.UI
{
    public class InputFooter : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private InputHint _inputHintPrefab;
        [SerializeField] private Transform _inputHintContainer;
        #endregion

        #region Private Fields
        private readonly List<InputHint> _inputHints = new();
        private readonly Dictionary<Guid, InputHint> _actionHintMap = new();
        private ObjectPool<InputHint> _inputHintPool;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            _inputHintPool = new ObjectPool<InputHint>(PoolCreateAction);
        }

        private void OnEnable()
        {
            InputManager inputManager = InputManager.Instance;
            inputManager.OnControlsChangedEvent += OnControlsChanged;
            inputManager.OnInputMapEnabledEvent += OnInputMapEnabled;
            inputManager.OnInputMapDisabledEvent += OnInputMapDisabled;
            inputManager.OnInputActionActivatedEvent += CreateInputHint;
            inputManager.OnInputActionDeactivatedEvent += RemoveInputHint;
            inputManager.OnInputActionEnabledEvent += ShowHint;
            inputManager.OnInputActionDisabledEvent += HideHint;
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.Instance;
            if (inputManager == null)
                return;

            inputManager.OnControlsChangedEvent -= OnControlsChanged;
            inputManager.OnInputMapEnabledEvent -= OnInputMapEnabled;
            inputManager.OnInputMapDisabledEvent -= OnInputMapDisabled;
            inputManager.OnInputActionActivatedEvent -= CreateInputHint;
            inputManager.OnInputActionDeactivatedEvent -= RemoveInputHint;
            inputManager.OnInputActionEnabledEvent -= ShowHint;
            inputManager.OnInputActionDisabledEvent -= HideHint;
        }

        private void OnDestroy()
        {
            _inputHintPool.Dispose();
        }
        #endregion

        #region Public Methods
        public void Clear()
        {
            foreach (InputHint hint in _inputHints)
                Destroy(hint.gameObject);

            _inputHints.Clear();
            _actionHintMap.Clear();
        }

        public void ChangeInputLabel(InputAction action, LocalizedString newLabel)
        {
            if (!TryGetInput(action, out InputHint hint))
                return;

            hint.SetInputText(newLabel);
        }
        #endregion

        #region Pooling
        private InputHint PoolCreateAction()
        {
            InputHint hint = Instantiate(_inputHintPrefab, _inputHintContainer);
            hint.Hide();
            return hint;
        }
        #endregion

        #region Private Methods
        private bool TryGetInput(InputAction action, out InputHint hint)
        {
            return _actionHintMap.TryGetValue(action.id, out hint);
        }

        private void CreateInputHint(InputAction action)
        {
            if (_actionHintMap.ContainsKey(action.id))
                return;

            InputHint inputHint = _inputHintPool.Get();
            inputHint.Action = action;
            inputHint.transform.SetAsLastSibling();
            _inputHints.Add(inputHint);
            _actionHintMap.Add(action.id, inputHint);
            inputHint.Show();
        }

        private void RemoveInputHint(InputAction action)
        {
            if (!TryGetInput(action, out InputHint hint))
                return;

            _inputHints.Remove(hint);
            _actionHintMap.Remove(action.id);
            hint.Hide();
            _inputHintPool.Release(hint);
        }

        private void ShowHint(InputAction action)
        {
            if (TryGetInput(action, out InputHint hint))
                hint.Show();
        }

        private void HideHint(InputAction action)
        {
            if (TryGetInput(action, out InputHint hint))
                hint.Hide();
        }

        private void OnControlsChanged()
        {
            foreach (InputHint hint in _inputHints)
                hint.RefreshIcon();
        }

        private void OnInputMapEnabled(InputActionMap inputMap)
        {
            foreach (InputAction action in inputMap.actions)
            {
                if (action.type != InputActionType.Button)
                    continue;

                CreateInputHint(action);
            }
        }

        private void OnInputMapDisabled(InputActionMap inputMap)
        {
            foreach (InputAction action in inputMap.actions)
            {
                if (action.type != InputActionType.Button)
                    continue;

                RemoveInputHint(action);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using Framework.Inputs.Data;
using Framework.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Framework.Inputs
{
    public class InputManager : GlobalSingleton<InputManager>
    {
        #region Serialized Fields
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private BindingsIconMapping _bindingsIconMapping;
        #endregion

        #region Private Fields
        private InputActionMap _currentInputMap;
        #endregion

        #region Public Properties
        public GameInputs InputActions { get; private set; }
        public string CurrentControlScheme => _playerInput.currentControlScheme;
        public IReadOnlyDictionary<string, Sprite> CurrentSchemeIconMapping => _bindingsIconMapping.BindingMapping[CurrentControlScheme];
        #endregion

        #region Events
        public event Action OnControlsChangedEvent = delegate { };
        public event Action<InputActionMap> OnInputMapEnabledEvent = delegate { };
        public event Action<InputActionMap> OnInputMapDisabledEvent = delegate { };
        public event Action<InputAction> OnInputActionActivatedEvent = delegate { };
        public event Action<InputAction> OnInputActionDeactivatedEvent = delegate { };
        public event Action<InputAction> OnInputActionEnabledEvent = delegate { };
        public event Action<InputAction> OnInputActionDisabledEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            //Switch back to C# class if I remove the PlayerInput component.
            //InputActions = new GameInputs();
            InputActions = new GameInputs(_playerInput.actions);
            InputActions.Disable();
        }

        private void OnEnable()
        {
            _playerInput.onControlsChanged += HandleOnControlsChanged;
        }

        private void OnDisable()
        {
            _playerInput.onControlsChanged -= HandleOnControlsChanged;
        }
        #endregion

        #region Override Methods
        protected override void OnOnDestroy()
        {
            base.OnOnDestroy();
            InputActions.Disable();
        }
        #endregion

        #region Public Methods
        public void EnableCurrentInputMap()
        {
            _currentInputMap?.Enable();
        }

        public void DisableCurrentInputMap()
        {
            _currentInputMap?.Disable();
        }

        public void EnableInputMap(InputActionMap inputMap)
        {
            inputMap?.Enable();
            OnInputMapEnabledEvent(inputMap);
        }

        public void DisableInputMap(InputActionMap inputMap)
        {
            inputMap?.Disable();
            OnInputMapDisabledEvent(inputMap);
        }

        public void ChangeCurrentInputMap(InputActionMap inputMapName)
        {
            DisableCurrentInputMap();
            _currentInputMap = inputMapName;
            EnableCurrentInputMap();
        }

        public InputActionMap FindActionMap(string mapName)
        {
            return InputActions.asset.FindActionMap(mapName);
        }

        public void EnableInput(InputAction inputAction)
        {
            inputAction?.Enable();
            OnInputActionEnabledEvent(inputAction);
        }

        public void DisableInput(InputAction inputAction)
        {
            inputAction?.Disable();
            OnInputActionDisabledEvent(inputAction);
        }

        public void ActivateActions(IReadOnlyCollection<SerializedAction> serializedActions)
        {
            if (serializedActions == null)
                return;

            foreach (SerializedAction serializedAction in serializedActions)
            {
                InputAction action = InputActions.FindAction(serializedAction.InputAction.action.id);
                action.Enable();
                action.performed += serializedAction.OnPerformed.Invoke;
                action.canceled += serializedAction.OnCanceled.Invoke;
                OnInputActionActivatedEvent(action);
            }
        }

        public void DeactivateActions(IReadOnlyCollection<SerializedAction> serializedActions)
        {
            if (serializedActions == null)
                return;

            foreach (SerializedAction serializedAction in serializedActions)
            {
                InputAction action = InputActions.FindAction(serializedAction.InputAction.action.id);
                action.Disable();
                action.performed -= serializedAction.OnPerformed.Invoke;
                action.canceled -= serializedAction.OnCanceled.Invoke;
                OnInputActionDeactivatedEvent(action);
            }
        }
        #endregion

        #region Private Methods
        private void HandleOnControlsChanged(PlayerInput input)
        {
            OnControlsChangedEvent();
        }
        #endregion
    }
}

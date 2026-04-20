using System;
using System.Collections.Generic;
using Framework.Inputs;
using Framework.Inputs.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class InputtableWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Cursor _cursor;
        [SerializeField] private List<SerializedAction> _availableInputs;
        #endregion

        #region Public Properties
        public bool IsActive { get; private set; }
        #endregion

        #region Events
        public Action OnActivatedEvent = delegate { };
        public Action OnDeactivatedEvent = delegate { };
        #endregion

        #region Public Methods
        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            if(_cursor != null)
                _cursor.gameObject.SetActive(true);
            EnableInputs();
            EnableSelectables(true);
            OnActivatedEvent();
        }

        public void Deactivate()
        {
            if(!IsActive)
                return;

            IsActive = false;
            if(_cursor != null)
                _cursor.gameObject.SetActive(false);
            EnableSelectables(false);
            DisableInputs();
            OnDeactivatedEvent();
        }

        public void DisableContent()
        {
            EnableSelectables(false);
        }
        #endregion

        #region Private Methods
        private void EnableSelectables(bool enable)
        {
            Selectable[] selectables = GetComponentsInChildren<Selectable>(true);
            foreach (Selectable item in selectables)
                item.enabled = enable;
        }

        private void EnableInputs()
        {
            InputManager inputManager = InputManager.Instance;
            inputManager.EnableInputMap(inputManager.InputActions.Menus);
            inputManager.ActivateActions(_availableInputs);
        }

        private void DisableInputs()
        {
            InputManager inputManager = InputManager.Instance;
            inputManager.DisableInputMap(inputManager.InputActions.Menus);
            inputManager.DeactivateActions(_availableInputs);
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        public void OnValidate()
        {
            if (_availableInputs == null)
                return;

            foreach (SerializedAction serializedAction in _availableInputs)
                serializedAction.Validate();
        }
#endif
#endregion
    }
}

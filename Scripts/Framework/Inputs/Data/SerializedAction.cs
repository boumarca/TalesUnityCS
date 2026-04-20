using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Framework.Inputs.Data
{
    [Serializable]
    public class SerializedAction
    {
        #region Serialized Fields
        //Editor only
        [HideInInspector][SerializeField] private string _name;
        #endregion

        #region Serialized Properties
        [field: SerializeField] public InputActionReference InputAction { get; private set; }
        [field: SerializeField] public UnityEvent<InputAction.CallbackContext> OnPerformed { get; private set; }
        [field: SerializeField] public UnityEvent<InputAction.CallbackContext> OnCanceled { get; private set; }
        #endregion

        #region Editor
#if UNITY_EDITOR
        public void Validate()
        {
            if (InputAction == null)
                return;

            _name = InputAction.name;
        }
#endif
        #endregion
    }
}

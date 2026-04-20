using UnityEngine;
using UnityEngine.InputSystem;

namespace Framework.Inputs
{
    public abstract class InputController : MonoBehaviour
    {
        #region MonoBehaviour Methods
        private void OnEnable()
        {
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }
        #endregion

        #region Abstract Methods
        public abstract void RegisterCallbacks();
        public abstract void UnregisterCallbacks();
        public abstract InputActionMap ActionMap { get; }
        #endregion

        #region Public Methods
        public void EnableInputs()
        {
            InputManager.Instance.EnableCurrentInputMap();
        }

        public void DisableInputs()
        {
            InputManager.Instance.DisableCurrentInputMap();
        }

        public void ChangeInputs()
        {
            InputManager.Instance.ChangeCurrentInputMap(ActionMap);
        }
        #endregion
    }
}

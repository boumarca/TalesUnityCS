using Framework.Inputs;
using UnityEngine.InputSystem;

namespace Jrpg.Maps.LocalMap
{
    public class LocalMapPlayerController : MapPlayerController
    {
        #region MapPlayerController Implementation
        public override void RegisterCallbacks()
        {
            GameInputs InputActions = InputManager.Instance.InputActions;
            InputActions.LocalMap.Move.performed += OnMovePerformed;
            InputActions.LocalMap.Move.canceled += OnMoveCanceled;
            InputActions.LocalMap.Interact.performed += OnInteractPerformed;
            InputActions.LocalMap.MainMenu.performed += OnOpenMainMenuPerformed;
        }

        public override void UnregisterCallbacks()
        {
            if (InputManager.Instance == null)
                return;

            GameInputs InputActions = InputManager.Instance.InputActions;
            InputActions.LocalMap.Move.performed -= OnMovePerformed;
            InputActions.LocalMap.Move.canceled -= OnMoveCanceled;
            InputActions.LocalMap.Interact.performed -= OnInteractPerformed;
            InputActions.LocalMap.MainMenu.performed -= OnOpenMainMenuPerformed;
        }
        public override InputActionMap ActionMap => InputManager.Instance.InputActions.LocalMap;

        #endregion

        #region Inputs callbacks
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            Player.Interact();
        }
        #endregion
    }
}

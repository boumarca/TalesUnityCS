using Framework.Inputs;
using Game.Maps.Actors;
using Game.StateStack;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jrpg.Maps
{
    public abstract class MapPlayerController : InputController
    {
        #region Serialized Fields
        [SerializeField] private StateData _mainMenuState;
        #endregion

        #region Public Properties
        public MapPlayer Player { get; set; }
        #endregion

        #region Inputs callbacks
        protected void OnMovePerformed(InputAction.CallbackContext context)
        {
            Player.Movement = context.ReadValue<Vector2>();
        }

        protected void OnMoveCanceled(InputAction.CallbackContext context)
        {
            Player.Movement = Vector2.zero;
        }

        protected void OnOpenMainMenuPerformed(InputAction.CallbackContext context)
        {
            StateStackManager.Instance.PushState(_mainMenuState);
        }
        #endregion
    }
}

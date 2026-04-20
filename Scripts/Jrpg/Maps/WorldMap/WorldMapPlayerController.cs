using Framework.Common;
using Framework.Inputs;
using Game.Maps.Actors;
using Jrpg.Maps.Vehicles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jrpg.Maps.WorldMap
{
    public class WorldMapPlayerController : MapPlayerController
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private VehicleManager _vehicleManager;
        #endregion

        #region Public Properties
        public Vector3 Position => Player.Position;
        public MapActor Actor => Player.Actor;
        public Direction CurrentDirection => Player.CurrentDirection;
        #endregion

        #region MapPlayerController Implementation
        //TODO: Add a common set of inputs for worldmap.
        public override void RegisterCallbacks()
        {
            GameInputs InputActions = InputManager.Instance.InputActions;
            InputActions.WorldMap.Move.performed += OnMovePerformed;
            InputActions.WorldMap.Move.canceled += OnMoveCanceled;
            InputActions.WorldMap.RideShip.performed += OnRideShipPerformed;
            InputActions.WorldMap.RideAirship.performed += OnRideAirshipPerformed;
            InputActions.WorldMap.MainMenu.performed += OnOpenMainMenuPerformed;

            InputActions.Ship.Move.performed += OnMoveVehiclePerformed;
            InputActions.Ship.Move.canceled += OnMoveVehicleCanceled;
            InputActions.Ship.RotateCamera.performed += OnRotateCameraPerformed;
            InputActions.Ship.RotateCamera.canceled += OnRotateCameraCanceled;
            InputActions.Ship.GetOffShip.performed += OnGetOffShipPerformed;
            InputActions.Ship.RideAirship.performed += OnRideAirshipPerformed;
            InputActions.Ship.MainMenu.performed += OnOpenMainMenuPerformed;

            InputActions.Airship.Move.performed += OnMoveVehiclePerformed;
            InputActions.Airship.Move.canceled += OnMoveVehicleCanceled;
            InputActions.Airship.RotateCamera.performed += OnRotateCameraPerformed;
            InputActions.Airship.RotateCamera.canceled += OnRotateCameraCanceled;
            InputActions.Airship.GetOffAirship.performed += OnGetOffAirshipPerformed;
            InputActions.Airship.MainMenu.performed += OnOpenMainMenuPerformed;
        }

        public override void UnregisterCallbacks()
        {
            if (InputManager.Instance == null)
                return;

            GameInputs InputActions = InputManager.Instance.InputActions;
            InputActions.WorldMap.Move.performed -= OnMovePerformed;
            InputActions.WorldMap.Move.canceled -= OnMoveCanceled;
            InputActions.WorldMap.RideShip.performed -= OnRideShipPerformed;
            InputActions.WorldMap.RideAirship.performed -= OnRideAirshipPerformed;
            InputActions.WorldMap.MainMenu.performed -= OnOpenMainMenuPerformed;

            InputActions.Ship.Move.performed -= OnMoveVehiclePerformed;
            InputActions.Ship.Move.canceled -= OnMoveVehicleCanceled;
            InputActions.Ship.RotateCamera.performed -= OnRotateCameraPerformed;
            InputActions.Ship.RotateCamera.canceled -= OnRotateCameraCanceled;
            InputActions.Ship.GetOffShip.performed -= OnGetOffShipPerformed;
            InputActions.Ship.RideAirship.performed -= OnRideAirshipPerformed;
            InputActions.Ship.MainMenu.performed -= OnOpenMainMenuPerformed;

            InputActions.Airship.Move.performed -= OnMoveVehiclePerformed;
            InputActions.Airship.Move.canceled -= OnMoveVehicleCanceled;
            InputActions.Airship.RotateCamera.performed -= OnRotateCameraPerformed;
            InputActions.Airship.RotateCamera.canceled -= OnRotateCameraCanceled;
            InputActions.Airship.GetOffAirship.performed -= OnGetOffAirshipPerformed;
            InputActions.Airship.MainMenu.performed -= OnOpenMainMenuPerformed;
        }

        public override InputActionMap ActionMap => InputManager.Instance.InputActions.WorldMap;
        #endregion

        #region Public Methods
        public void ChangeToWorldMapInputs()
        {
            InputManager.Instance.ChangeCurrentInputMap(ActionMap);
        }

        public void ChangeToShipInputs()
        {
            InputManager.Instance.ChangeCurrentInputMap(InputManager.Instance.InputActions.Ship);
        }

        public void ChangeToAirshipInputs()
        {
            InputManager.Instance.ChangeCurrentInputMap(InputManager.Instance.InputActions.Airship);
        }
        #endregion

        #region Inputs Callbacks
        private void OnMoveVehiclePerformed(InputAction.CallbackContext context)
        {
            _vehicleManager.CurrentVehicle.Movement = context.ReadValue<Vector2>();
        }

        private void OnMoveVehicleCanceled(InputAction.CallbackContext context)
        {
            _vehicleManager.CurrentVehicle.Movement = Vector2.zero;
        }

        private void OnRotateCameraPerformed(InputAction.CallbackContext context)
        {
            _vehicleManager.CurrentVehicle.CameraRotation = context.ReadValue<Vector2>();
        }

        private void OnRotateCameraCanceled(InputAction.CallbackContext context)
        {
            _vehicleManager.CurrentVehicle.CameraRotation = Vector2.zero;
        }

        private void OnRideShipPerformed(InputAction.CallbackContext context)
        {
            _vehicleManager.RideShipFromLand();
        }

        private void OnRideAirshipPerformed(InputAction.CallbackContext context)
        {
            _vehicleManager.RideAirship();
        }

        private void OnGetOffShipPerformed(InputAction.CallbackContext context)
        {
            _vehicleManager.GetOffVehicle();
        }

        private void OnGetOffAirshipPerformed(InputAction.CallbackContext context)
        {
            _vehicleManager.GetOffAirship();
        }
        #endregion
    }
}

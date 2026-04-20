using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Cameras;
using Game.Maps.WorldMap;
using Jrpg.Maps.Data;
using Jrpg.Maps.WorldMap;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jrpg.Maps.Vehicles
{
    public class VehicleManager : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component references")]
        [FormerlySerializedAs("_overworldManager")][SerializeField] private WorldMapManager _worldMapManager;
        [SerializeField] private WorldMapPlayerController _playerController;
        [SerializeField] private Mode7Camera _playerCamera;

        [Header("Vehicles")]
        [SerializeField] private VehicleBase _ship;
        [SerializeField] private VehicleBase _airship;
        #endregion

        #region Private Fields
        private WorldMapRoot _worldMap;
        #endregion

        #region Public Properties
        public VehicleBase CurrentVehicle { get; private set; }
        #endregion

        #region Public Methods
        public void Initialize(WorldMapRoot worldMap)
        {
            _worldMap = worldMap;
        }

        public void StartOnVehicle(string vehicleId)
        {
            if (vehicleId == _ship.name)
                ChangeVehicle(_ship);
            else if (vehicleId == _airship.name)
                ChangeVehicle(_airship);
        }

        public void RideShipFromLand()
        {
            if (!_ship.CanEmbark(_worldMap, _playerController.Position, _playerController.CurrentDirection))
                return;

            StartVehicleTransition(_ship);
        }

        public void RideShip()
        {
            StartVehicleTransition(_ship);
        }

        public void RideAirship()
        {
            StartVehicleTransition(_airship);
        }

        public void GetOffAirship()
        {
            IEnumerable<SuperTile> tiles = _worldMap.GetTilesAtWorldPosition(CurrentVehicle.Position);
            if (tiles.Any(tile => tile.IsTerrainType(TerrainType.Sea)))
                RideShip();
            else
                GetOffVehicle();
        }

        public void GetOffVehicle()
        {
            if (!CurrentVehicle.CanDisembark(_worldMap))
                return;

            StartVehicleTransition(null);
        }
        #endregion

        #region Private Methods
        private void StartVehicleTransition(VehicleBase vehicle)
        {
            StartCoroutine(ChangeVehicleTransition(vehicle));
        }

        private IEnumerator ChangeVehicleTransition(VehicleBase vehicle)
        {
            yield return ScreenCamera.Instance.FadeOut();
            ChangeVehicle(vehicle);
            yield return ScreenCamera.Instance.FadeIn();
        }

        private void ChangeVehicle(VehicleBase vehicle)
        {
            GetOffCurrentVehicle();
            CurrentVehicle = vehicle;
            RideNewVehicle(vehicle);
            _worldMapManager.CurrentPlayerActor = vehicle != null ? vehicle.Actor : _playerController.Actor;
        }

        private void GetOffCurrentVehicle()
        {
            if (CurrentVehicle == null)
                return;

            Vector2 position = CurrentVehicle.Disembark();
            _playerController.Actor.Teleport(_worldMap.GetCellCenterWorld(position), CurrentVehicle.CurrentDirection);
            _playerCamera.Activate();
            _playerController.Actor.Show();
            _playerController.ChangeToWorldMapInputs();
        }

        private void RideNewVehicle(VehicleBase vehicle)
        {
            if (vehicle == null)
                return;

            CurrentVehicle.Embark(_worldMap.GetCellCenterWorld(_playerController.Position), _playerController.Actor.CurrentDirection);
            _playerCamera.Deactivate();
            _playerController.Actor.Hide();

            if (CurrentVehicle == _airship)
                _playerController.ChangeToAirshipInputs();
            else if (CurrentVehicle == _ship)
                _playerController.ChangeToShipInputs();
        }
        #endregion
    }
}

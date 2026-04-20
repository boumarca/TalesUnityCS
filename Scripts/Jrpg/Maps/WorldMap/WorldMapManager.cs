using System.Threading.Tasks;
using Game.Maps;
using Game.Maps.Actors;
using Game.Maps.Data;
using Game.Maps.WorldMap;
using Jrpg.Maps.Data;
using Jrpg.Maps.Vehicles;
using UnityEngine;

namespace Jrpg.Maps.WorldMap
{
    public class WorldMapManager : MapManager
    {
        #region Serialized Fields
        [SerializeField] private VehicleManager _vehicleManager;
        #endregion

        #region Public Properties
        public WorldMapRoot WorldMap { get; private set; }
        public MapActor CurrentPlayerActor { get; set; }
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            if (CurrentPlayerActor == null)
                return;

            Vector3 playerPosition = CurrentPlayerActor.Position;
            Vector3 newPlayerPosition = WorldMap.ClampLoopToMap(CurrentPlayerActor.Position);
            if (newPlayerPosition != playerPosition)
                CurrentPlayerActor.Teleport(newPlayerPosition);

            WorldMap.UpdateMapToPlayer(newPlayerPosition);
        }
        #endregion

        #region MapManager Implementation
        public override bool AllowSave => true;

        public override MapSaveData SaveMapData()
        {
            MapSaveData data = base.SaveMapData();
            if (_vehicleManager.CurrentVehicle != null)
            {
                VehicleBase vehicle = _vehicleManager.CurrentVehicle;
                MapActor vehicleActor = vehicle.Actor;
                data.VehicleId = vehicle.name;
                data.PlayerPositionX = vehicleActor.Position.x;
                data.PlayerPositionY = vehicleActor.Position.y;
                data.PlayerOrientation = vehicleActor.CurrentDirection;
            }
            return data;
        }

        public override async Task InitializeMap(DestinationInfo destinationInfo)
        {
            if (destinationInfo.MapId != OverworldMapId)
            {
                Debug.LogWarning("Changing MapId to OverworldMapId for some reason");
                destinationInfo.MapId = OverworldMapId;
            }

            await base.InitializeMap(destinationInfo);
            CurrentPlayerActor = Player.Actor;
            WorldMap = CurrentMap as WorldMapRoot;
            _vehicleManager.Initialize(WorldMap);
            string vehicleId = destinationInfo.VehicleId;
            if (!string.IsNullOrEmpty(vehicleId))
            {
                _vehicleManager.StartOnVehicle(vehicleId);
                VehicleBase vehicle = _vehicleManager.CurrentVehicle;
                vehicle.MoveTo(destinationInfo.SpawnPosition);
                vehicle.Actor.ChangeOrientation(destinationInfo.SpawnOrientation);
            }
        }

        protected override void HandleTeleporterEvent(object sender, TeleporterEventArgs eventArgs)
        {
            TeleportPlayerToLocalMap(eventArgs.Destination);
        }
        #endregion

        #region Private Methods
        private void TeleportPlayerToLocalMap(DestinationInfo destinationInfo)
        {
            PlayerController.DisableInputs();
            //TODO: Use the LoadMap function
            MapStateManager.Instance.LoadLocalMap(destinationInfo);
        }
        #endregion
    }
}

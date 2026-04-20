using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Extensions;
using Framework.Singleton;
using Game.Cameras;
using Game.Maps;
using Game.Maps.Data;
using Game.SaveSystem;
using Game.StateStack;
using Jrpg.Maps.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jrpg.Maps
{
    //TODO: Convert or merge into a GameStateManager
    public class MapStateManager : GlobalSingleton<MapStateManager>, ISaveable
    {
        #region Serialized Fields
        [Header("Asset References")]
        [SerializeField] private StateData _localMapState;
        [SerializeField] private StateData _worldMapState;

        [Header("Default data")]
        [SerializeField] private SceneId _startingScene;
        [SerializeField] private DestinationId _startingSpawnPoint;
        #endregion

        #region Private Fields
        private bool _isLoading;
        private MapManager _mapManager;
        #endregion

        #region Public Properties
        public MapRoot CurrentMap => _mapManager.CurrentMap;
        public bool AllowSave => _mapManager.AllowSave;
        #endregion

        #region Events
        public event EventHandler<EventArgs> OnMapStateChangedEvent = delegate { };
        #endregion

        #region ISaveable Implementation
        public IEnumerable<Type> SaveDataTypes => new[] { typeof(MapSaveData) };

        public bool TryLoadData(SaveDataBase saveData)
        {
            if (saveData is not MapSaveData mapSaveData)
                return false;

            DestinationInfo destinationInfo = new()
            {
                MapId = new SceneId(mapSaveData.CurrentMapId),
                SubmapName = mapSaveData.CurrentSubMap,
                SpawnPosition = new Vector2(mapSaveData.PlayerPositionX, mapSaveData.PlayerPositionY),
                SpawnOrientation = mapSaveData.PlayerOrientation,
                VehicleId = mapSaveData.VehicleId
            };

            LoadMap(destinationInfo, true);
            return true;
        }

        public SaveDataBase SaveData()
        {
            return _mapManager.SaveMapData();
        }
        #endregion

        #region Public Methods
        public void LoadDefaultMap()
        {
            DestinationInfo defaultDestination = new()
            {
                MapId = _startingScene,
                SpawnPointId = _startingSpawnPoint
            };

            LoadMap(defaultDestination, true);
        }

        public void LoadMap(DestinationInfo destinationInfo, bool fromSaveData = false)
        {
            if (destinationInfo.MapId == MapManager.OverworldMapId)
                LoadWorldMap(destinationInfo, fromSaveData);
            else
                LoadLocalMap(destinationInfo, fromSaveData);
        }

        public void LoadLocalMap(DestinationInfo destinationInfo, bool fromSaveData = false)
        {
            if (_isLoading)
                return;

            ChangeSceneAsync(_localMapState, destinationInfo, fromSaveData).FireAndForget();
        }

        public void LoadWorldMap(DestinationInfo destinationInfo, bool fromSaveData = false)
        {
            if (_isLoading)
                return;

            ChangeSceneAsync(_worldMapState, destinationInfo, fromSaveData).FireAndForget();
        }
        #endregion

        #region Private Methods
        private async Task ChangeSceneAsync(StateData newState, DestinationInfo destinationInfo, bool fromSaveData)
        {
            _isLoading = true;
            await ScreenCamera.Instance.FadeOut();
            await LoadNewState(newState, destinationInfo);
            await ScreenCamera.Instance.FadeIn();
            if(!fromSaveData)
                OnMapStateChangedEvent(this, EventArgs.Empty);
            _isLoading = false;
        }

        private async Task LoadNewState(StateData newState, object payload)
        {
            await StateStackManager.Instance.ChangeToStateAsync(newState, payload);
            _mapManager = FindAnyObjectByType<MapManager>();
        }
        #endregion
    }
}

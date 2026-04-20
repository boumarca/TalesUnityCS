using System;
using System.Threading.Tasks;
using Framework.Common;
using Framework.Identifiers;
using Game.Cameras;
using Game.Maps;
using Game.Maps.Actors;
using Game.Maps.Data;
using Game.StateStack;
using Jrpg.Maps.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Jrpg.Maps
{
    public abstract class MapManager : GameStateBehaviour
    {
        #region Constants
        public static readonly SceneId OverworldMapId = new("Overworld");
        #endregion

        #region Serialized Fields
        [Header("Asset References")]
        [SerializeField] private AssetReferenceGameObject _playerAsset;

        [Header("Component References")]
        [SerializeField] private MapPlayerController _playerController;
        [SerializeField] private FollowerCamera _followerCamera;
        #endregion

        #region Private Fields
        private AsyncOperationHandle<GameObject> _playerHandle;
        private AsyncOperationHandle<SceneInstance> _currentSceneHandle;
        private SceneId _currentSceneId;
        #endregion

        #region Protected Properties
        protected MapPlayer Player { get; private set; }
        protected MapPlayerController PlayerController => _playerController;
        protected FollowerCamera Camera => _followerCamera;
        #endregion

        #region Public Properties
        public MapRoot CurrentMap { get; private set; }
        public virtual bool AllowSave => true;
        #endregion

        #region MonoBehaviour Methods
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ReleaseHandles();
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            if (payload is DestinationInfo destinationInfo)
                return InitializeMap(destinationInfo);

            PlayerController.ChangeInputs();
            return Task.CompletedTask;
        }

        public override void OnExitState()
        {
            PlayerController.DisableInputs();
        }

        public override void OnSuspendState()
        {
            PlayerController.DisableInputs();
        }

        public override void OnResumeState()
        {
            PlayerController.ChangeInputs();
        }
        #endregion

        #region Virtual Methods
        public virtual async Task InitializeMap(DestinationInfo destinationInfo)
        {
            await LoadNewScene(destinationInfo.MapId);
            await LoadPlayer();
            PlayerController.Player = Player;
            Camera.Follower = Player.Actor.transform;
            TeleportPlayer(destinationInfo);
            PlayerController.ChangeInputs();
        }

        public virtual MapSaveData SaveMapData()
        {
            return new MapSaveData
            {
                CurrentMapId = _currentSceneId.Id,
                CurrentSubMap = CurrentMap.CurrentSubMap.name,
                PlayerPositionX = Player.Position.x,
                PlayerPositionY = Player.Position.y,
                PlayerOrientation = Player.CurrentDirection
            };
        }

        protected virtual void HandleTeleporterEvent(object sender, TeleporterEventArgs eventArgs) { }
        #endregion

        #region Protected Methods
        protected void TeleportPlayer(DestinationInfo destinationInfo)
        {
            if (Identifier.IsValid(destinationInfo.SpawnPointId))
            {
                (SubMap subMap, SpawnPoint spawnPoint) = CurrentMap.GetSubMapWithSpawnPoint(destinationInfo.SpawnPointId);
                Direction orientation = spawnPoint.KeepOrientation ? Player.CurrentDirection : spawnPoint.Orientation;
                TeleportPlayerToPosition(subMap, spawnPoint.Position, orientation);
            }
            else
            {
                SubMap submap = CurrentMap.GetSubMapByName(destinationInfo.SubmapName);
                TeleportPlayerToPosition(submap, destinationInfo.SpawnPosition, destinationInfo.SpawnOrientation);
            }
        }

        protected async Task LoadNewScene(SceneId sceneId)
        {
            if (sceneId == _currentSceneId)
                return;

            if (_currentSceneHandle.IsValid())
            {
                AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(_currentSceneHandle);
                await unloadHandle.Task;
            }

            AsyncOperationHandle<SceneInstance> sceneHandle = Addressables.LoadSceneAsync(sceneId.Name, LoadSceneMode.Additive);
            await sceneHandle.Task;

            CurrentMap = FindAnyObjectByType<MapRoot>();
            CurrentMap.Initialize();
            CurrentMap.RegisterEvent(HandleTeleporterEvent);
            _currentSceneId = sceneId;
            _currentSceneHandle = sceneHandle;
        }
        #endregion

        #region Private Methods
        private async Task LoadPlayer()
        {
            _playerHandle = _playerAsset.InstantiateAsync();
            await _playerHandle.Task;
            Player = _playerHandle.Result.GetComponent<MapPlayer>();
        }

        private void TeleportPlayerToPosition(SubMap subMap, Vector2 position, Direction direction)
        {
            CurrentMap.ChangeSubMap(subMap);
            Player.Actor.Teleport(position, direction);
        }

        private void ReleaseHandles()
        {
            if (_playerHandle.IsValid())
                Addressables.Release(_playerHandle);

            if (_currentSceneHandle.IsValid())
                Addressables.Release(_currentSceneHandle);
        }
        #endregion
    }
}

using System;
using Game.Maps.Data;
using UnityEngine;

namespace Game.Maps
{
    public class Teleporter : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private TriggerZone _triggerZone;

        [Header("Game Data")]
        [SerializeField] private SceneId _mapId;
        [SerializeField] private DestinationId _spawnPointId;

        [HideInInspector][SerializeField] private string _sceneId;
        [HideInInspector][SerializeField] private string _sceneIdRaw;
        [HideInInspector][SerializeField] private string _destinationId;
        [HideInInspector][SerializeField] private string _destinationIdRaw;
        #endregion

        #region Private Fields
        private TeleporterEventArgs _teleporterEventArgs;
        private DestinationInfo _destinationInfo;
        #endregion

        #region Events
        public event EventHandler<TeleporterEventArgs> OnTeleportEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            _destinationInfo = new DestinationInfo()
            {
                MapId = _mapId,
                SpawnPointId = _spawnPointId
            };
            _teleporterEventArgs = new TeleporterEventArgs(_destinationInfo);
        }
        #endregion

        #region Public Methods
        public void Teleport()
        {
            OnTeleportEvent(this, _teleporterEventArgs);
        }
        #endregion

        #region Private Methods
        private void HandleOnTrigger(object sender, EventArgs eventArgs)
        {
            Teleport();
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateMapId();
            ValidateSpawnPointId();
        }

        private void ValidateMapId()
        {
            if (_sceneId == _sceneIdRaw)
                return;

            _sceneIdRaw = _sceneId;
            _mapId = new SceneId(_sceneId);
        }

        private void ValidateSpawnPointId()
        {
            if (_destinationId == _destinationIdRaw)
                return;

            _destinationIdRaw = _destinationId;
            _spawnPointId = new DestinationId(_destinationId);
        }
#endif
        #endregion
    }
}

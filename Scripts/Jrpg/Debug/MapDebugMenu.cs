using Framework.Identifiers;
using Game.Maps.Data;
using Jrpg.Maps;
using UnityEngine;

namespace Jrpg.Debug
{
    public class MapDebugMenu : MonoBehaviour
    {
        [SerializeField] private SceneId _mapId;
        [SerializeField] private DestinationId _spawnPointId;

        public bool IsTeleportValid() => Identifier.IsValid(_mapId) && Identifier.IsValid(_spawnPointId);

        public void CheatTeleportToScene()
        {
            if(Identifier.IsNullOrEmpty(_mapId))
            {
                UnityEngine.Debug.LogError($"{nameof(_mapId)} is empty");
                return;
            }

            if (Identifier.IsNullOrEmpty(_spawnPointId))
            {
                UnityEngine.Debug.LogError($"{nameof(_spawnPointId)} is empty");
                return;
            }

            DestinationInfo destinationInfo = new DestinationInfo()
            {
                MapId = _mapId,
                SpawnPointId = _spawnPointId
            };

            MapStateManager.Instance.LoadMap(destinationInfo);
        }
    }
}

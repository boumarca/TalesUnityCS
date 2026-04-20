using Framework.Common;
using UnityEngine;

namespace Game.Maps.Data
{
    public class DestinationInfo
    {
        #region Public Properties
        public SceneId MapId { get; set; }
        public DestinationId SpawnPointId { get; set; }
        public string SubmapName { get; set; }
        public Vector2 SpawnPosition { get; set; }
        public Direction SpawnOrientation { get; set; }
        public string VehicleId { get; set; }
        #endregion
    }
}

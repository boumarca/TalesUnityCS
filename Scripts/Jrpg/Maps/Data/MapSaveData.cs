using System;
using Framework.Common;
using Game.SaveSystem;

namespace Jrpg.Maps.Data
{
    [Serializable]
    public class MapSaveData : SaveDataBase
    {
        #region Public Properties
        public string CurrentMapId { get; set; }
        public string CurrentSubMap { get; set; }
        public float PlayerPositionX { get; set; }
        public float PlayerPositionY { get; set; }
        public Direction PlayerOrientation { get; set; }
        public string VehicleId { get; set; }
        #endregion
    }
}

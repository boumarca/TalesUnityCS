using Framework.Common;
using Game.Maps.WorldMap;
using UnityEngine;

namespace Jrpg.Maps.Vehicles
{
    public class WorldMapAirship : VehicleBase
    {
        #region Static Fields
        private static readonly RaycastHit2D[] s_hitsBuffer = new RaycastHit2D[4];
        #endregion

        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Rigidbody2D _rigidbody2D;

        [Header("Game Parameters")]
        [SerializeField] private float _heightOffset = 5;
        #endregion

        #region VehicleBaseController Override
        public override void Embark(Vector3 position, Direction direction)
        {
            position += Vector3.back * _heightOffset;
            base.Embark(position, Direction.Up);
        }

        public override Vector3 Disembark()
        {
            Actor.ChangeOrientation(Direction.Down);
            return base.Disembark();
        }

        public override bool CanDisembark(WorldMapRoot currentMap)
        {
            int hits = _rigidbody2D.Cast(Vector3.forward, s_hitsBuffer, _heightOffset * 2);
            return hits == 0;
        }

        public override void MoveTo(Vector3 position)
        {
            position += Vector3.back * _heightOffset;
            base.MoveTo(position);
        }
        #endregion
    }
}

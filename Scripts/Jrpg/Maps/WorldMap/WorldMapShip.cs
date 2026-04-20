using System.Collections.Generic;
using System.Linq;
using Framework.Common;
using Game.Maps.WorldMap;
using Jrpg.Maps.Data;
using SuperTiled2Unity;
using UnityEngine;

namespace Jrpg.Maps.Vehicles
{
    public class WorldMapShip : VehicleBase
    {
        #region VehicleBaseController Override
        public override bool CanEmbark(WorldMapRoot currentMap, Vector3 position, Direction direction)
        {
            IEnumerable<SuperTile> tiles = currentMap.GetTilesAtWorldPosition(position);
            foreach (SuperTile tile in tiles)
            {
                if (!tile.IsTerrainType(TerrainType.Beach))
                    continue;

                Vector3 frontTilePosition = position + direction.ToVector3();
                IEnumerable<SuperTile> frontTiles = currentMap.GetTilesAtWorldPosition(frontTilePosition);
                if (frontTiles.Any(frontTile => frontTile.IsTerrainType(TerrainType.Sea)))
                    return true;
            }
            return false;
        }

        public override bool CanDisembark(WorldMapRoot currentMap)
        {
            //Assumes the ship is on a Sea tile.
            Vector3 frontTilePosition = Position + GetWorldFacingDirection().ToVector3();
            IEnumerable<SuperTile> frontTiles = currentMap.GetTilesAtWorldPosition(frontTilePosition);
            return frontTiles.Any(frontTile => frontTile.IsTerrainType(TerrainType.Beach));
        }

        public override void Embark(Vector3 position, Direction direction)
        {
            //Assumes the distance between tiles is one unit.
            Vector3 offsettedPosition = position + direction.ToVector3();
            base.Embark(offsettedPosition, direction);
        }

        public override Vector3 Disembark()
        {
            Direction facingDirection = GetWorldFacingDirection();
            Actor.Teleport(Position + facingDirection.ToVector3());
            Vector3 disembarkPosition = base.Disembark();
            Actor.ChangeOrientation(facingDirection);
            return disembarkPosition;
        }
        #endregion

        #region Private Functions
        private Direction GetWorldFacingDirection()
        {
            Vector3 relativeDirection = Camera.ToRelativeDirection(CurrentDirection.ToVector2());
            return DirectionExtensions.FromVector2(relativeDirection);
        }
        #endregion
    }
}

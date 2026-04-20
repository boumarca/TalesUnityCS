using Jrpg.Maps.Data;
using SuperTiled2Unity;

namespace Jrpg.Maps
{
    public static class SuperTileExtensions
    {
        #region Static Functions
        public static bool IsTerrainType(this SuperTile tile, TerrainType terrainType)
        {
            return tile.GetPropertyValueAsEnum<TerrainType>("_terrainType") == terrainType;
        }
        #endregion
    }
}

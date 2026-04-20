using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Maps.WorldMap
{
    public class WorldMapChunk : SubMap
    {
        #region Serialized Fields
        [Header("Components References")]
        [SerializeField] private Transform _transform;
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap[] _tilemaps;
        #endregion

        #region Private Fields
        private Vector3 _startingPosition;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            _startingPosition = _transform.position;
        }
        #endregion

        #region Public Methods

        public void OffsetPosition(Vector3 offset)
        {
            _transform.position = _startingPosition + offset;
        }

        public Vector3Int GetWorldToCellIndex(Vector3 position)
        {
            Vector3Int cellIndex = _grid.WorldToCell(position);
            //Make sure that z is always 0.
            cellIndex.z = 0;
            //The Tiled tilemaps have a -1 y offset, so I correct the value before returning.
            return cellIndex + Vector3Int.up;
        }

        public Vector3 GetCellCenterWorld(Vector3 position)
        {
            Vector3 worldPosition = _grid.GetCellCenterWorld(_grid.WorldToCell(position));
            //Make sure that z is always 0.
            worldPosition.z = 0;
            return worldPosition;
        }

        public IList<SuperTile> GetTilesAtCellIndex(Vector3Int cellIndex)
        {
            List<SuperTile> tiles = new();
            foreach (Tilemap tilemap in _tilemaps)
            {
                SuperTile tile = tilemap.GetTile<SuperTile>(cellIndex);
                if (tile != null)
                    tiles.Add(tile);
            }
            return tiles;
        }

        public IList<SuperTile> GetTilesAtWorldPosition(Vector3 worldPosition)
        {
            return GetTilesAtCellIndex(GetWorldToCellIndex(worldPosition));
        }
        #endregion

        #region Editor
        private void Reset()
        {
            _transform = transform;
        }
        #endregion
    }
}

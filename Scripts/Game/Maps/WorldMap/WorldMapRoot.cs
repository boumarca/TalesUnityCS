using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;

namespace Game.Maps.WorldMap
{
    public class WorldMapRoot : MapRoot
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Transform _transform;

        [Header("Game Data")]
        [SerializeField] private WorldMapChunk[] _mapChunks;
        [SerializeField] private Vector2Int _chunkCount;
        #endregion

        #region Private Fields
        private Vector2Int _currentChunkCoord = -Vector2Int.one;
        private Vector2 _chunkSize;
        private Vector2 _bottomLeftBounds;
        private Vector2 _topRightBounds;
        #endregion

        #region Public Properties
        public WorldMapChunk CurrentMapChunk { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        #endregion

        #region MapRoot Implementation
        protected override IReadOnlyCollection<SubMap> SubMaps => _mapChunks;

        protected override void InitializeMap()
        {
            CurrentMapChunk = CurrentSubMap as WorldMapChunk;
            //All computations assumes that chunks have the same size.
            _chunkSize = new Vector3(_mapChunks[0].Width, _mapChunks[0].Height);
            Width = _chunkCount.x * _chunkSize.x;
            Height = _chunkCount.y * _chunkSize.y;
            ComputeMapBoundaries();
        }
        #endregion

        #region Public Methods
        public void UpdateMapToPlayer(Vector3 playerPosition)
        {
            int x = Mathf.FloorToInt(playerPosition.x / _chunkSize.x);
            int y = Mathf.FloorToInt(-playerPosition.y / _chunkSize.y);
            Vector2Int newCoord = new(x, y);
            if (newCoord == _currentChunkCoord)
                return;

            _currentChunkCoord = newCoord;
            CurrentMapChunk = _mapChunks[ToChunkIndex(_currentChunkCoord)];
            UpdateChunkVisibility();
            MoveLoopedChunks();
        }

        public Vector3 GetCellCenterWorld(Vector3 position)
        {
            return CurrentMapChunk.GetCellCenterWorld(position);
        }

        public IEnumerable<SuperTile> GetTilesAtWorldPosition(Vector3 worldPosition)
        {
            return CurrentMapChunk.GetTilesAtWorldPosition(worldPosition);
        }

        public Vector3 ClampLoopToMap(Vector3 playerPosition)
        {
            Vector3 loopOffset = Vector3.zero;

            if (playerPosition.x < _bottomLeftBounds.x)
                loopOffset = Vector3.right * Width;
            else if (playerPosition.x > _topRightBounds.x)
                loopOffset = Vector3.left * Width;
            else if (playerPosition.y < _bottomLeftBounds.y)
                loopOffset = Vector3.up * Height;
            else if (playerPosition.y > _topRightBounds.y)
                loopOffset = Vector3.down * Height;

            return playerPosition + loopOffset;
        }
        #endregion

        #region Private Methods
        private void ComputeMapBoundaries()
        {
            Vector2 mapPosition = _transform.position;

            float xMin = mapPosition.x;
            float xMax = mapPosition.x + Width;

            //Map goes into -y
            float yMin = mapPosition.y - Height;
            float yMax = mapPosition.y;

            _bottomLeftBounds = new Vector2(xMin, yMin);
            _topRightBounds = new Vector2(xMax, yMax);
        }

        private int ToChunkIndex(Vector2Int chunkCoord)
        {
            return chunkCoord.y * _chunkCount.y + chunkCoord.x;
        }

        private Vector2Int ToChunkCoord(int chunkIndex)
        {
            int x = chunkIndex % _chunkCount.x;
            int y = chunkIndex / _chunkCount.y;
            return new Vector2Int(x, y);
        }

        private void UpdateChunkVisibility()
        {
            Vector2Int currentCoord = _currentChunkCoord;
            Vector2Int currentCoordLooped = _currentChunkCoord + _chunkCount;
            for (int i = 0; i < _mapChunks.Length; i++)
            {
                Vector2Int chunkCoord = ToChunkCoord(i);
                Vector2Int chunkCoordLooped = chunkCoord + _chunkCount;
                bool isLoopedX = chunkCoord.x == currentCoordLooped.x - 1 || chunkCoordLooped.x - 1 == currentCoord.x;
                bool isLoopedY = chunkCoord.y == currentCoordLooped.y - 1 || chunkCoordLooped.y - 1 == currentCoord.y;
                bool isVisible = (chunkCoord.x >= currentCoord.x - 1 && chunkCoord.x <= currentCoord.x + 1 || isLoopedX)
                    && (chunkCoord.y >= currentCoord.y - 1 && chunkCoord.y <= currentCoord.y + 1 || isLoopedY);

                if (isVisible)
                    _mapChunks[i].ActivateMap();
                else
                    _mapChunks[i].DeactivateMap();
            }
        }

        private void MoveLoopedChunks()
        {
            for (int i = 0; i < _mapChunks.Length; i++)
            {
                WorldMapChunk chunk = _mapChunks[i];
                Vector3 offset = Vector3.zero;
                Vector2Int chunkCoord = ToChunkCoord(i);

                if (_currentChunkCoord.x >= _chunkCount.x / 2 && chunkCoord.x == 0)
                    offset += Vector3.right * Width;
                else if (_currentChunkCoord.x < _chunkCount.x / 2 && chunkCoord.x == _chunkCount.x - 1)
                    offset += Vector3.left * Width;

                if (_currentChunkCoord.y >= _chunkCount.y / 2 && chunkCoord.y == 0)
                    offset += Vector3.down * Height;
                else if (_currentChunkCoord.y < _chunkCount.y / 2 && chunkCoord.y == _chunkCount.y - 1)
                    offset += Vector3.up * Height;

                chunk.OffsetPosition(offset);
            }
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void Reset()
        {
            _transform = transform;
        }

        private void OnValidate()
        {
            _mapChunks = GetComponentsInChildren<WorldMapChunk>(true);
        }
#endif
        #endregion
    }
}

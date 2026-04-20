using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace Game.Cameras
{
    public class LocalMapCamera : FollowerCamera
    {
        #region Serialized Fields
        [Header("Component references")]
        [SerializeField] private PixelPerfectCamera _pixelPerfectCamera;
        #endregion

        #region Private Fields
        private Vector2 _bottomLeftBounds;
        private Vector2 _topRightBounds;
        #endregion

        #region FollowerCamera Implementation
        protected override void OnLateUpdate()
        {
            ClampToBounds();
        }
        #endregion

        #region Public Methods
        public void ChangeBounds(Vector2 center, int width, int height)
        {
            ComputeBounds(center, new Vector2(width, height));
        }
        #endregion

        #region Private Methods
        private void ClampToBounds()
        {
            Vector2 position = Follower.position;

            if (position.x < _bottomLeftBounds.x)
                position.x = _bottomLeftBounds.x;
            else if (position.x > _topRightBounds.x)
                position.x = _topRightBounds.x;

            if (position.y < _bottomLeftBounds.y)
                position.y = _bottomLeftBounds.y;
            else if (position.y > _topRightBounds.y)
                position.y = _topRightBounds.y;

            Followee.position = position;
        }

        private void ComputeBounds(Vector2 mapPosition, Vector2 mapSize)
        {
            float viewportWidth = (float)_pixelPerfectCamera.refResolutionX / _pixelPerfectCamera.assetsPPU;
            float viewportHeight = (float)_pixelPerfectCamera.refResolutionY / _pixelPerfectCamera.assetsPPU;

            float xMin = mapPosition.x + viewportWidth / 2;
            float xMax = mapPosition.x + mapSize.x - viewportWidth / 2;

            //Map goes into -y
            float yMin = mapPosition.y - (mapSize.y - viewportHeight / 2);
            float yMax = mapPosition.y - viewportHeight / 2;

            _bottomLeftBounds = new Vector2(xMin, yMin);
            _topRightBounds = new Vector2(xMax, yMax);
        }
        #endregion
    }
}

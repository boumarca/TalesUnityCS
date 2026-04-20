using Framework.Common;
using UnityEngine;

namespace Game.Cameras
{
    public class Mode7Camera : FollowerCamera
    {
        #region Serialized Fields
        [Header("Component references")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Game Parameters")]
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private bool _invertXRotationAxis = true;
        [SerializeField] private bool _invertYRotationAxis = true;
        [SerializeField] private float _defaultCameraAngle;
        [SerializeField] private MinMaxValue _yCameraRange;
        #endregion

        #region Private Fields
        private float _yCameraRotation;
        #endregion

        #region Public Properties
        public Vector3 Right => Followee.right;
        //We use the back world vector because the world plane is vertical.
        public Vector3 WorldForward => Vector3.Cross(Followee.right, Vector3.back).normalized;
        #endregion

        #region MonoBehaviour Methods
        protected override void OnFollowerChanged()
        {
            ResetCamera();
        }

        protected override void OnLateUpdate()
        {
            FaceCamera();
        }
        #endregion

        #region Public Methods
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            ResetCamera();
        }

        public void Rotate(Vector2 rotation)
        {
            Vector3 position = Follower.position;
            float deltaX = rotation.x * _rotationSpeed * Time.deltaTime;
            float deltaY = rotation.y * _rotationSpeed * Time.deltaTime;

            if (_invertXRotationAxis)
                deltaX *= -1;

            if (_invertYRotationAxis)
                deltaY *= -1;

            Followee.RotateAround(position, Vector3.forward, deltaX);

            if (!_yCameraRange.IsInRange(_yCameraRotation + deltaY))
                return;

            Followee.RotateAround(position, Followee.right, deltaY);
            _yCameraRotation += deltaY;
        }

        public Vector3 ToRelativeDirection(Vector3 direction)
        {
            return direction.x * Right + direction.y * WorldForward;
        }
        #endregion

        #region Private Methods
        private void ResetCamera()
        {
            Followee.rotation = Quaternion.identity;
            _yCameraRotation = _defaultCameraAngle;
            Followee.RotateAround(transform.position, Followee.right, _yCameraRotation);
        }

        private void FaceCamera()
        {
            if (Follower == null || _rotationSpeed <= 0)
                return;

            Follower.rotation = _cameraTransform.rotation;
        }
        #endregion
    }
}

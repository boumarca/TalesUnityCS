using UnityEngine;

namespace Game.Cameras
{
    public class FollowerCamera : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component references")]
        [SerializeField] private Transform _transform;
        [SerializeField] private Camera _camera;
        #endregion

        #region Private Fields
        private Transform _follower;
        #endregion

        #region Protected Properties
        protected Transform Followee => _transform;
        #endregion

        #region Public Properties
        public Transform Follower
        {
            get => _follower;
            set
            {
                _follower = value;
                OnFollowerChanged();
            }
        }
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            _camera.targetTexture = ScreenCamera.Instance.WorldRenderTexture;
        }

        private void LateUpdate()
        {
            if (Follower == null)
                return;

            FollowObject();
            OnLateUpdate();
        }
        #endregion

        #region Virtual Methods
        protected virtual void OnFollowerChanged() { }
        protected virtual void OnLateUpdate() { }
        #endregion

        #region Private Methods
        private void FollowObject()
        {
            Followee.position = Follower.position;
        }
        #endregion
    }
}

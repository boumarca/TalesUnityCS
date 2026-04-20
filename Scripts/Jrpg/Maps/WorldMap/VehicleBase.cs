using Framework.Common;
using Game.Cameras;
using Game.Maps.Actors;
using Game.Maps.WorldMap;
using UnityEngine;

namespace Jrpg.Maps.Vehicles
{
    public class VehicleBase : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Transform _transform;
        [SerializeField] private Mode7Camera _camera;
        [SerializeField] private MapActor _actor;
        #endregion

        #region Protected Proterties
        protected Mode7Camera Camera => _camera;
        #endregion

        #region Public Properties
        public MapActor Actor => _actor;
        public Direction CurrentDirection => _actor.CurrentDirection;
        public Vector2 Movement { get; set; }
        public Vector2 CameraRotation { get; set; }
        public Vector3 Position => _actor.Position;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            Camera.Follower = _transform;
        }

        private void FixedUpdate()
        {
            MoveActor(Movement);
        }

        private void LateUpdate()
        {
            RotateCamera(CameraRotation);
        }
        #endregion

        #region Virtual Methods
        public virtual void Embark(Vector3 position, Direction direction)
        {
            Actor.Teleport(position);
            Actor.ChangeOrientation(direction);
            gameObject.SetActive(true);
            Camera.Activate();
        }

        public virtual Vector3 Disembark()
        {
            gameObject.SetActive(false);
            Camera.Deactivate();
            return Position;
        }

        public virtual bool CanEmbark(WorldMapRoot currentMap, Vector3 position, Direction direction)
        {
            return true;
        }

        public virtual bool CanDisembark(WorldMapRoot currentMap)
        {
            return true;
        }

        public virtual void MoveTo(Vector3 position)
        {
            Actor.Teleport(position);
        }
        #endregion

        #region Private Methods
        private void MoveActor(Vector2 movement)
        {
            Vector2 relativeMovement = Camera.ToRelativeDirection(movement);
            Actor.Move(relativeMovement.normalized, movement);
        }

        private void RotateCamera(Vector2 rotation)
        {
            Camera.Rotate(rotation);
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void Reset()
        {
            _transform = transform;
        }
#endif
        #endregion
    }
}

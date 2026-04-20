using System;
using Framework.Common;
using Framework.Utils;
using UnityEngine;

namespace Game.Maps.Data
{
    public class SpawnPoint : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Transform _transform;

        [Header("Parameters")]
        [SerializeField] private DestinationId _destinationId;
        [SerializeField] private bool _keepOrientation;
        [SerializeField][HideIf(nameof(_keepOrientation), true)] private Direction _orientationDirection;

        [HideInInspector][SerializeField] private string _id;
        [HideInInspector][SerializeField] private string _idRaw;
        [HideInInspector][SerializeField] private string _orientation;
        [HideInInspector][SerializeField] private string _orientationRaw;
        #endregion

        #region Public Properties
        public DestinationId Id => _destinationId;
        public Vector2 Position => _transform.position;
        public bool KeepOrientation => _keepOrientation;
        public Direction Orientation => _orientationDirection;
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void Reset()
        {
            _transform = transform;
        }

        private void OnValidate()
        {
            ValidateOrientation();
            ValidateId();
        }

        private void ValidateOrientation()
        {
            if (_orientation == _orientationRaw)
                return;

            _orientationRaw = _orientation;
            if (!Enum.TryParse(_orientationRaw, true, out _orientationDirection))
                _orientationDirection = Direction.None;
        }

        private void ValidateId()
        {
            if (_id == _idRaw)
                return;

            _idRaw = _id;
            _destinationId = new DestinationId(_id);
        }
#endif
        #endregion
    }
}

using Framework.Animation;
using Framework.Assertions;
using Game.Maps.Actors;
using UnityEngine;

namespace Jrpg.Maps.Vehicles
{
    public class AirshipShadow : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Transform _transform;
        [SerializeField] private MapActor _airship;
        [SerializeField] private SimpleAnimator _simpleAnimator;

        [Header("Animations")]
        [SerializeField] private DirectionalAnimations _animations;
        #endregion

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            _transform.SetParent(null);
        }

        private void LateUpdate()
        {
            if (!_airship.gameObject.activeInHierarchy)
            {
                _transform.SetParent(_airship.transform);
                return;
            }

            Vector3 airshipPosition = _airship.Position;
            _transform.SetPositionAndRotation(new Vector3(airshipPosition.x, airshipPosition.y, 0), _airship.Rotation);
            SimpleAnimation anim = _animations.GetAnimationForDirection(_airship.CurrentDirection);
            AssertWrapper.IsNotNull(anim, $"Missing animation for direction {_airship.CurrentDirection} for {_airship.name}");
            _simpleAnimator.PlayAnimation(anim);
        }
        #endregion

#if UNITY_EDITOR
        private void Reset()
        {
            _transform = transform;
        }
#endif
    }
}

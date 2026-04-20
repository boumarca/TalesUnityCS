using Framework.Animation;
using Framework.Assertions;
using Framework.Common;
using UnityEngine;

namespace Game.Maps.Actors
{
    public class MapActor : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Transform _transform;
        [SerializeField] private Rigidbody2D _rigidbody2d;
        [SerializeField] private SimpleAnimator _simpleAnimator;
        [SerializeField] private Collider2D _collider2D;

        [Header("Animations")]
        [SerializeField] private DirectionalAnimations _idleAnimations;
        [SerializeField] private DirectionalAnimations _walkAnimations;

        [Header("Game Parameters")]
        [SerializeField] private float _moveSpeed = 5;
        [SerializeField] private Direction _defaultDirection = Direction.Down;
        #endregion

        #region Public Properties
        public Collider2D Collider => _collider2D;
        public Vector3 Position => _transform.position;
        public Quaternion Rotation => _transform.rotation;
        public Direction CurrentDirection { get; private set; }
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            if(_defaultDirection != Direction.None)
                CurrentDirection = _defaultDirection;
        }

        private void LateUpdate()
        {
            if (_rigidbody2d.linearVelocity == Vector2.zero)
                PlayIdleAnimation();
            else
                PlayWalkAnimation();
        }
        #endregion

        #region Public Methods
        public void Move(Vector2 movement, Vector2 direction)
        {
            _rigidbody2d.linearVelocity = _moveSpeed * movement;

            if (direction == Vector2.zero)
                return;

            if (Vector2.Dot(CurrentDirection.ToVector2(), direction) > 0)
                return;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                CurrentDirection = direction.x > 0 ? Direction.Right : Direction.Left;
            else
                CurrentDirection = direction.y > 0 ? Direction.Up : Direction.Down;
        }

        public void Teleport(Vector3 position)
        {
            transform.position = position;
        }

        public void Teleport(Vector3 position, Direction newOrientation)
        {
            Teleport(position);
            ChangeOrientation(newOrientation);
        }

        public void ChangeOrientation(Direction newOrientation)
        {
            CurrentDirection = newOrientation;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        #endregion

        #region Private Methods
        private void PlayIdleAnimation()
        {
            SimpleAnimation anim = _idleAnimations.GetAnimationForDirection(CurrentDirection);
            AssertWrapper.IsNotNull(anim, $"Missing idle animation for direction {CurrentDirection} for {name}");
            _simpleAnimator.PlayAnimation(anim);
        }

        private void PlayWalkAnimation()
        {
            SimpleAnimation anim = _walkAnimations.GetAnimationForDirection(CurrentDirection);
            AssertWrapper.IsNotNull(anim, $"Missing walk animation for direction {CurrentDirection} for {name}");
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

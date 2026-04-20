using UnityEngine;

namespace Framework.Physics
{
    public class PhysicsObject : MonoBehaviour
    {
        #region Static Variables
        private static readonly RaycastHit2D[] s_hitBuffer = new RaycastHit2D[16];
        private static readonly ContactPoint2D[] s_contactBuffer = new ContactPoint2D[16];
        #endregion

        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] Rigidbody2D _rigidbody2d;
        [SerializeField] Collider2D _collider2d;

        [Header("Game parameters")]
        [SerializeField] float _gravityModifier = 8f;
        [SerializeField] float _minGroundNormalY = 0.65f;
        [SerializeField] float _shellRadius = 0.01f;
        [SerializeField] float _minMoveDistance = 0.001f;
        [SerializeField] float _separationSmoothFactor = 4f;
        #endregion

        #region Private Variables
        private Vector2 _groundNormal;
        private ContactFilter2D _contactFilter;
        private bool _enableGravity = true;
        #endregion

        #region Public Properties
        public Vector2 Velocity { get; private set; }
        public Vector2 TargetVelocity { get; private set; }
        public bool IsGrounded { get; private set; }
        #endregion

        #region  MonoBehaviour Methods
        private void Start()
        {
            _contactFilter.useTriggers = false;
        }

        private void FixedUpdate()
        {
            _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            ResolveOverlaps();

            Vector2 newVelocity = Velocity;
            newVelocity += _enableGravity ? _gravityModifier * Time.deltaTime * Physics2D.gravity : Vector2.zero;
            newVelocity.x = TargetVelocity.x;
            Velocity = newVelocity;

            IsGrounded = false;
            Vector2 deltaPosition = Velocity * Time.deltaTime;
            Vector2 normal = IsGrounded ? _groundNormal : Vector2.up;
            Vector2 moveAlongNormal = new(normal.y, -normal.x);
            Vector2 move = moveAlongNormal * deltaPosition.x;

            Movement(move, false);
            move = Vector2.up * deltaPosition.y;
            Movement(move, true);
        }
        #endregion

        #region Public Methods
        public void MoveToPosition(Vector2 position)
        {
            _rigidbody2d.position = position;
        }

        public void ForceVelocity(Vector2 velocity)
        {
            TargetVelocity = velocity;
            Velocity = velocity;
        }

        public void EnableGravity(bool enable)
        {
            _enableGravity = enable;
        }
        #endregion

        #region Private Methods
        private void Movement(Vector2 move, bool yMovement)
        {
            float distance = move.magnitude;
            if (distance > _minMoveDistance)
            {
                int count = _rigidbody2d.Cast(move, _contactFilter, s_hitBuffer, distance + _shellRadius);
                for (int i = 0; i < count; i++)
                {
                    Vector2 currentNormal = s_hitBuffer[i].normal;
                    if (currentNormal.y > _minGroundNormalY /*&& s_hitBuffer[i].collider.gameObject.layer == (int)Layers.Ground*/) //Add back once I generate the layers enum
                    {
                        IsGrounded = true;
                        if (yMovement)
                        {
                            _groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }

                    float projection = Vector2.Dot(Velocity, currentNormal);
                    if (projection < 0)
                    {
                        Velocity -= projection * currentNormal;
                    }

                    float modifiedDistance = s_hitBuffer[i].distance - _shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }

            _rigidbody2d.position += move.normalized * distance;
        }

        private void ResolveOverlaps()
        {
            int count = _collider2d.GetContacts(_contactFilter, s_contactBuffer);
            Vector2 newPosition = _rigidbody2d.position;
            for (int i = 0; i < count; i++)
            {
                ContactPoint2D contact = s_contactBuffer[i];
                newPosition -= contact.normal * (contact.separation / _separationSmoothFactor);
            }
            _rigidbody2d.position = newPosition;
        }
        #endregion
    }
}

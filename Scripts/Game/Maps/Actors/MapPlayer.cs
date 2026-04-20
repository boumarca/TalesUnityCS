using System.Collections.Generic;
using Framework.Common;
using Framework.Extensions;
using Framework.Generated;
using UnityEngine;

namespace Game.Maps.Actors
{
    public class MapPlayer : MonoBehaviour
    {
        #region Static Fields
        private static readonly List<Collider2D> s_interactionResults = new(1);
        #endregion

        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private MapActor _actor;
        [SerializeField] private Collider2D _collider2D;

        [Header("Game Data")]
        [SerializeField] private float _interactionDistance = 1;
        #endregion

        #region Private Fields
        private ContactFilter2D _filter;
        #endregion

        #region Public Properties
        public MapActor Actor => _actor;
        public Vector3 Position => _actor.Position;
        public Direction CurrentDirection => _actor.CurrentDirection;
        public Vector2 Movement { get; set; }
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            _filter = new ContactFilter2D();
            _filter.SetLayerMask(Layers.MapInteractions.ToLayerMask());
            _filter.useTriggers = false;
        }

        private void FixedUpdate()
        {
            _actor.Move(Movement, Movement);
        }
        #endregion

        #region Public Methods
        public void Interact()
        {
            Vector2 interactionCenter = (Vector2)Position + (CurrentDirection.ToVector2() * _interactionDistance);
            int results = Physics2D.OverlapBox(interactionCenter, Vector2.one * 0.5f, 0, _filter, s_interactionResults);
            if (results == 0)
                return;

            foreach (Collider2D result in s_interactionResults)
            {
                //TODO: Add priority to overlapping objects.
                IMapInteractable[] mapInteractables = result.GetComponentsInChildren<IMapInteractable>();
                foreach (IMapInteractable interactable in mapInteractables)
                    interactable.Interact(this);
            }
        }

        public void EnableCollisions()
        {
            _collider2D.enabled = true;
        }

        public void DisableCollisions()
        {
            _collider2D.enabled = false;
        }
        #endregion
    }
}

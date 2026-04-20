#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using Framework.Extensions;
using Framework.Identifiers;
using UnityEngine;
using UnityEngine.Events;
using Game.Maps.Actors;

namespace Game.Maps
{
    public class TriggerZone : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Collider2D _collider2d;

        [Header("GameData")]
        [SerializeField] private Tag[] _triggerTags;

        [Header("Events")]
        [SerializeField] private UnityEvent _onTriggerUnityEvent;
        [SerializeField] private UnityEvent _onStopTriggerUnityEvent;
        #endregion

        #region Private Fields
        private bool _hasCollision;
        #endregion

        #region MonoBehaviour Methods
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (_hasCollision)
                return;

            if (_triggerTags.None(triggerTag => collision.CompareTag(triggerTag.Name)))
                return;

            if (!collision.TryGetComponent(out MapActor actor))
                return;

            if (!IsInsideTrigger(actor.Collider))
                return;

            _hasCollision = true;
            _onTriggerUnityEvent.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _hasCollision = false;
            _onStopTriggerUnityEvent.Invoke();
        }
        #endregion

        #region Private Methods
        private bool IsInsideTrigger(Collider2D other)
        {
            Bounds thisBounds = _collider2d.bounds;
            Bounds otherBounds = other.bounds;

            return thisBounds.Contains(otherBounds.min) && thisBounds.Contains(otherBounds.max);
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        public void ResizeCollider(Vector3 size)
        {
            transform.localScale = size;
        }

        public void RegisterPersistentEvent(UnityAction methodCall)
        {
            UnityEventTools.AddPersistentListener(_onTriggerUnityEvent, methodCall);
        }
#endif
        #endregion
    }
}

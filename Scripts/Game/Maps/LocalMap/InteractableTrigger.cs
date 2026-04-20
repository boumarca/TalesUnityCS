using Game.Maps.Actors;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Maps.LocalMap
{
    public class InteractableTrigger : MonoBehaviour, IMapInteractable
    {
        #region Serialized Fields
        [SerializeField] private UnityEvent _onInteraction;
        #endregion

        #region IMapInteractable Implementation
        public void Interact(MapPlayer player)
        {
            _onInteraction.Invoke();
        }
        #endregion
    }
}

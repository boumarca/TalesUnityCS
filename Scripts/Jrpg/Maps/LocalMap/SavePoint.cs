using Game.StateStack;
using UnityEngine;

namespace Jrpg.Maps.LocalMap
{
    public class SavePoint : MonoBehaviour
    {
        #region Statics
        private static readonly int s_openTrigger = Animator.StringToHash("Open");
        private static readonly int s_closeTrigger = Animator.StringToHash("Close");
        #endregion

        #region Serialized Fields
        [SerializeField] private StateData _saveMenuState;
        [SerializeField] private Animator _animator;
        #endregion

        #region Unity Event Methods
        public void OnInteraction()
        {
            StateStackManager.Instance.PushState(_saveMenuState);
        }

        public void OnPlayerInsideTrigger()
        {
            _animator.SetTrigger(s_openTrigger);
        }

        public void OnPlayerExitTrigger()
        {
            _animator.SetTrigger(s_closeTrigger);
        }
        #endregion
    }
}

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.StateStack
{
    public abstract class GameStateBehaviour : MonoBehaviour
    {
        #region Private Fields
        private AsyncOperationHandle _handle;
        #endregion

        #region Public Properties
        public StateData StateData { get; private set; }
        #endregion

        #region MonoBehaviour Methods
        protected virtual void OnDestroy()
        {
            if (_handle.IsValid())
                Addressables.Release(_handle);
        }
        #endregion

        #region Abstract Methods
        public abstract Task OnEnterState(object payload);
        public abstract void OnExitState();
        public abstract void OnSuspendState();
        public abstract void OnResumeState();
        #endregion

        #region Internal Methods
        internal void InitializeState(StateData stateData, AsyncOperationHandle handle)
        {
            StateData = stateData;
            _handle = handle;
        }
        #endregion
    }
}

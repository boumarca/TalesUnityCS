using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Extensions;
using Framework.Singleton;
using UnityEngine;

namespace Game.StateStack
{
    public class StateStackManager : GlobalSingleton<StateStackManager>
    {
        #region Private Fields
        private readonly Stack<GameStateBehaviour> _stateStack = new();
#if UNITY_EDITOR
        private List<GameStateBehaviour> _debugStack;
#endif
        #endregion

        #region Properties
        private GameStateBehaviour _currentState;
        #endregion

        #region Public Methods
        /// <summary>
        /// Go to the default state provided as parameter.
        /// </summary>
        /// <param name="defaultState"></param>
        public Task InitializeAsync(StateData defaultState)
        {
            return LoadStateAsync(defaultState, null);
        }

        /// <summary>
        /// Change the current state without stacking.
        /// Fire and forget the task.
        /// </summary>
        /// <param name="newStateData"></param>
        /// <param name="payload"></param>
        public void ChangeToState(StateData newStateData, object payload = null)
        {
            ChangeToStateAsync(newStateData, payload).FireAndForget();
        }

        /// <summary>
        /// Change the current state without stacking.
        /// </summary>
        /// <param name="newStateData"></param>
        /// <param name="payload"></param>
        public Task ChangeToStateAsync(StateData newStateData, object payload = null)
        {
            _currentState.OnExitState();
            return LoadStateAsync(newStateData, payload);
        }

        /// <summary>
        /// Push the new state to the stack with payload.
        /// Fire and forget the task.
        /// </summary>
        /// <param name="newStateData"></param>
        /// <param name="payload"></param>
        public void PushState(StateData newStateData, object payload = null)
        {
            PushStateAsync(newStateData, payload).FireAndForget();
        }

        /// <summary>
        /// Push the new state to the stack with payload.
        /// </summary>
        /// <param name="newStateData"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        ///
        public Task PushStateAsync(StateData newStateData, object payload = null)
        {
            _currentState.OnSuspendState();
            _stateStack.Push(_currentState);
            return LoadStateAsync(newStateData, payload);
        }

        /// <summary>
        /// Return to the previous state
        /// </summary>
        public void ReturnToPreviousState()
        {
            if (_stateStack.Count == 0)
            {
                Debug.LogWarning("No state to return back to. Operation cancelled");
                return;
            }

            _currentState.OnExitState();
            _currentState = _stateStack.Pop();
            _currentState.OnResumeState();
            Debug.Log($"[StackState] New State {_currentState}");
#if UNITY_EDITOR
            _debugStack = _stateStack.ToList();
#endif
        }

        /// <summary>
        /// Return to the previous state provided by the StateData
        /// The operation will fail if the State is not in the Stack.
        /// </summary>
        /// <param name="stateData">State to return to.</param>
        public void ReturnToPreviousState(StateData stateData)
        {
            if (_stateStack.Count == 0)
            {
                Debug.LogWarning("No state to return back to. Operation cancelled");
                return;
            }

            if(_stateStack.None(state => state.StateData == stateData))
            {
                Debug.LogWarning($"The state {stateData} is not in the Stack. Operation cancelled");
                return;
            }

            _currentState.OnExitState();

            while (_currentState.StateData != stateData)
                _currentState = _stateStack.Pop();

            _currentState.OnResumeState();
            Debug.Log($"[StackState] New State {_currentState}");
#if UNITY_EDITOR
            _debugStack = _stateStack.ToList();
#endif
        }

        /// <summary>
        /// Clear the state stack.
        /// </summary>
        public void ClearStack()
        {
            _stateStack.Clear();
        }
        #endregion

        #region Private Methods
        private async Task LoadStateAsync(StateData newStateData, object payload)
        {
            _currentState = await newStateData.LoadState();
            await _currentState.OnEnterState(payload);
            Debug.Log($"[StackState] New State {_currentState}");
#if UNITY_EDITOR
            _debugStack = _stateStack.ToList();
#endif
        }
        #endregion
    }
}

using System.Threading.Tasks;
using UnityEngine;

namespace Game.StateStack
{
    public abstract class StateData : ScriptableObject
    {
        #region Abstract Methods
        public abstract Task<GameStateBehaviour> LoadState();
        #endregion
    }
}

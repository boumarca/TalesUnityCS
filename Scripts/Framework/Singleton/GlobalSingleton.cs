using UnityEngine;

namespace Framework.Singleton
{
    public abstract class GlobalSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        #region Singleton Implementation
        protected override void OnAwake()
        {
            base.OnAwake();
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}

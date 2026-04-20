using UnityEngine;

namespace Framework.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Public Properties
        //CA1000: Do not declare static members on generic types
        public static T Instance { get; private set; }
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            GameObject target = gameObject;
            if (Instance != null)
            {
                Destroy(target);
                return;
            }

            Instance = target.GetComponent<T>();

            OnAwake();
        }

        private void OnDestroy()
        {
            OnOnDestroy();
            if (Instance == this)
                Instance = null;
        }
        #endregion

        #region Virtual Methods
        protected virtual void OnAwake() { }
        protected virtual void OnOnDestroy() { }
        #endregion
    }
}

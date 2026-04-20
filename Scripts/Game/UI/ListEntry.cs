using System;
using UnityEngine;

namespace Game.UI
{
    public class ListEntry : MonoBehaviour
    {
        #region Public Properties
        public object Data { get; private set; }
        #endregion

        #region Events
        public event Action<object> OnConfirmItemEvent = delegate { };
        #endregion

        #region Public Methods
        public void Initialize(object data)
        {
            Data = data;
            OnInitialize();
        }
        #endregion

        #region Protected Methods
        protected virtual void OnInitialize() { }
        #endregion

        #region Unity Event UI Methods
        public void UIOnClicked()
        {
            OnConfirmItemEvent(Data);
        }
        #endregion
    }
}

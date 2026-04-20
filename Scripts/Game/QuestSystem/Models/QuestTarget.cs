using System;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.QuestSystem.Models
{
    /// <summary>
    /// This class represents an object which can be the target of a quest step.
    /// </summary>
    public class QuestTarget : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private QuestTargetId _targetId;
        #endregion

        #region Events
        public event EventHandler OnTargetInteractedEvent = delegate { };
        #endregion

        #region Public Properties
        public QuestTargetId TargetId => _targetId;
        #endregion

        #region Public Method
        public void Interact()
        {
            OnTargetInteractedEvent(this, EventArgs.Empty);
        }
        #endregion
    }
}

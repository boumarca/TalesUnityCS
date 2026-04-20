using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.QuestSystem.Data
{
    [Serializable]
    public class QuestPreconditionsList : IEnumerable<QuestPrecondition>
    {
        #region Serialized Fields
        [SerializeField] private QuestPrecondition[] _questPreconditions;
        #endregion

        #region Public Methods
        public IEnumerator<QuestPrecondition> GetEnumerator()
        {
            foreach (QuestPrecondition precondition in _questPreconditions)
                yield return precondition;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _questPreconditions.GetEnumerator();
        }
        #endregion
    }
}

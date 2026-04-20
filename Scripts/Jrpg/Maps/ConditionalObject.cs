using System.Linq;
using Game.Maps;
using Game.QuestSystem;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Jrpg.Maps
{
    public class ConditionalObject : MapObject
    {
        #region Serialized Fields
        [Header("Game Data")]
        [SerializeField] private QuestPreconditionsList _preconditions;
        #endregion

        #region Override Methods
        public override void Refresh()
        {
            RefreshActiveState();
        }
        #endregion

        #region Private Methods
        private void RefreshActiveState()
        {
            bool activeState = _preconditions.All(precondition => QuestManager.Instance.IsPreconditionMet(precondition));
            gameObject.SetActive(activeState);
        }
        #endregion
    }
}

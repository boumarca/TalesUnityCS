using Framework.Extensions;
using Framework.Identifiers;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using UnityEngine;

namespace Jrpg.Debug
{
    public class PartyDebugMenu : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ActorId _actorId;
        [SerializeField] private int _valueToAdd;
        [SerializeField] private Vector2Int _swapIndexes; 
        #endregion

        #region Properties
        public RpgParty CurrentParty => PartyManager.Instance.CurrentParty;
        public ActorId SelectedActor => _actorId;
        public int ValueToAdd => _valueToAdd;
        public Vector2Int SwapIndexes => _swapIndexes;
        #endregion

        #region Public Methods
        public bool AreSwapIndexesValid()
        {
            return _swapIndexes.x.IsBetween(0, CurrentParty.Members.Count - 1)
                && _swapIndexes.y.IsBetween(0, CurrentParty.Members.Count - 1)
                && _swapIndexes.x != _swapIndexes.y;
        }

        public void CheatAddPartyMember()
        {
            if(IsActorValid())
                PartyManager.Instance.AddPartyMember(_actorId);
        }

        public void CheatRemovePartyMember()
        {
            if (IsActorValid())
                PartyManager.Instance.RemovePartyMember(_actorId);
        }

        public void CheatGainExperience()
        {
            if(_valueToAdd > 0)
                PartyManager.Instance.GainExperience(_valueToAdd);
        }

        public void CheatSwapPartyMembers()
        {
            if (AreSwapIndexesValid())
                PartyManager.Instance.SwapMembers(_swapIndexes.x, _swapIndexes.y);
        }

        public void CheatGainMoney()
        {
            PartyManager.Instance.GainMoney(_valueToAdd);
        }
        #endregion

        #region Private Methods
        private bool IsActorValid()
        {
            if (Identifier.IsNullOrEmpty(_actorId))
            {
                UnityEngine.Debug.LogError($"{nameof(_actorId)} is empty");
                return false;
            }
            return true;
        }
        #endregion
    }
}

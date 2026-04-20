using System.Collections.Generic;
using Framework.Identifiers;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using UnityEngine;

namespace Jrpg.Debug
{
    public class ActorDebugMenu : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ActorId _actorId;
        [SerializeField] private int _cheatValue;
        #endregion

        #region Properties
        public ActorId SelectedActor => _actorId;
        public int CheatValue => _cheatValue;
        #endregion

        #region Public Methods
        public void CheatGiveExperienceToActor()
        {
            if (IsActorValid() && _cheatValue > 0)
                ActorManager.Instance.GiveExperienceToActor(_actorId, _cheatValue);
        }

        public void CheatChangeHp()
        {
            if (!IsActorValid())
                return;

            RpgActor actor = ActorManager.Instance.GetActor(_actorId);
            actor.CurrentHp += _cheatValue;
        }

        public RpgActor GetActor(ActorId id)
        {
            if (IsActorValid())
                return ActorManager.Instance.GetActor(id);
            return null;
        }

        public IEnumerable<RpgActor> ActorEnumerator()
        {
            ActorManager actorManager = ActorManager.Instance;
            foreach(ActorId actorId in actorManager.ActorIds)
                yield return actorManager.GetActor(actorId);
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

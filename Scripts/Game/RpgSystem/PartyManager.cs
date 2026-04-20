using System;
using System.Collections.Generic;
using Framework.Singleton;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.SaveSystem;
using UnityEngine;

namespace Game.RpgSystem
{
    public class PartyManager : GlobalSingleton<PartyManager>, ISaveable
    {
        #region Serialized Fields
        [SerializeField] private RpgActorData _defaultActor;
        #endregion

        #region Private Fields
        private RpgParty _party = new();
        #endregion

        #region Public Properties
        public RpgParty CurrentParty => _party;
        #endregion

        #region ISaveable Implementation
        public IEnumerable<Type> SaveDataTypes => new[] { typeof(PartySaveData) };
        public bool TryLoadData(SaveDataBase saveData)
        {
            if (saveData is not PartySaveData partySaveData)
                return false;

            InitializeFromSaveData(partySaveData);
            return true;
        }

        public SaveDataBase SaveData()
        {
            PartySaveData saveData = new();
            foreach (RpgActor actor in _party.Members)
                saveData.MemberIds.Add(actor.Id.Id);

            return saveData;
        }
        #endregion

        #region Public Methods
        public void InitializeAsNew()
        {
            InitializeParty();
        }

        public void AddPartyMember(ActorId actorId)
        {
            RpgActor actor = ActorManager.Instance.GetActor(actorId);
            _party.AddPartyMember(actor);
        }

        public void RemovePartyMember(ActorId actorId)
        {
            RpgActor actor = ActorManager.Instance.GetActor(actorId);
            _party.RemovePartyMember(actor);
        }

        public void GainExperience(int amount)
        {
            _party.GainExperience(amount);
        }

        public void GainMoney(int amount)
        {
            _party.GainMoney(amount);
        }

        public void SwapMembers(int firstIndex, int secondIndex)
        {
            _party.Swap(firstIndex, secondIndex);
        }
        #endregion

        #region Private Methods
        private void InitializeParty()
        {
            _party.Clear();
            AddPartyMember(_defaultActor.Id);
        }

        private void InitializeFromSaveData(PartySaveData partySaveData)
        {
            InitializeAsNew();
            foreach (string id in partySaveData.MemberIds)
            {
                ActorId actorId = new(id);
                AddPartyMember(actorId);
            }
        }
        #endregion
    }
}

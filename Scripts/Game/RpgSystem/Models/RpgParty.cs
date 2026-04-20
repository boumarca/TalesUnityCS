using System;
using System.Collections.Generic;
using Framework.Assertions;
using Framework.Extensions;

namespace Game.RpgSystem.Models
{
    public class RpgParty
    {
        #region Constants
        public const int MaxActivePartySize = 4;
        #endregion

        #region Private Fields
        private List<RpgActor> _members = new();
        private List<RpgActor> _activeMembers = new();
        private List<RpgActor> _reserveMembers = new();
        #endregion

        #region Public Properties
        public IReadOnlyList<RpgActor> Members => _members;
        public IReadOnlyList<RpgActor> ActiveMembers => _activeMembers;
        public IReadOnlyList<RpgActor> ReserveMembers => _reserveMembers;
        public int Money { get; private set; }
        public bool HasReserve => _reserveMembers.Count > 0;
        #endregion

        #region Public Methods
        public void AddPartyMember(RpgActor actor)
        {
            if (_members.Contains(actor))
                return;

            _members.Add(actor);
            if(_activeMembers.Count < MaxActivePartySize)
                _activeMembers.Add(actor);
            else
                _reserveMembers.Add(actor);
        }

        public void RemovePartyMember(RpgActor actor)
        {
            if (_members.Count == 1)
                return;

            _ = _members.Remove(actor);
            RefreshMemberLists();
        }

        public void Swap(int firstIndex, int secondIndex)
        {
            AssertWrapper.IsIndexInRange(firstIndex, _members, $"{nameof(firstIndex)} is out of {nameof(_members)}'s range");
            AssertWrapper.IsIndexInRange(secondIndex, _members, $"{nameof(secondIndex)} is out of {nameof(_members)}'s range");
            (_members[firstIndex], _members[secondIndex]) = (_members[secondIndex], _members[firstIndex]);
            RefreshMemberLists();
        }

        public void GainExperience(int amount)
        {
            foreach (RpgActor rpgActor in _members)
                rpgActor.GainExperience(amount);
        }

        public void GainMoney(int amount)
        {
            Money = Math.Max(0, Money + amount);
        }

        public void Clear()
        {
            _members.Clear();
            _activeMembers.Clear();
            _reserveMembers.Clear();
            Money = 0;
        }
        #endregion

        #region Private Methods
        private void RefreshMemberLists()
        {
            _activeMembers.Clear();
            _reserveMembers.Clear();
            for (int i = 0; i < _members.Count; i++)
            {
                if(i < MaxActivePartySize)
                    _activeMembers.Add(_members[i]);
                else
                    _reserveMembers.Add(_members[i]);
            }
        }
        #endregion
    }
}

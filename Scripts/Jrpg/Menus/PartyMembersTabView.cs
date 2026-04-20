using System;
using System.Collections.Generic;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;

namespace Jrpg.Menus
{
    public class PartyMembersTabView : TabList
    {
        #region Serialized Fields
        [SerializeField] private ActorTab _tabPrefab;
        #endregion

        #region Events
        public event Action<RpgActor> OnTabChangedEvent = delegate { };
        #endregion

        #region Public Methods
        public void PopulateTabs()
        {
            IReadOnlyList<RpgActor> actors = PartyManager.Instance.CurrentParty.Members;
            foreach (RpgActor actor in actors)
            {
                ActorTab actorTab = Instantiate(_tabPrefab);
                actorTab.SetActor(actor);
                actorTab.OnTabSelectedEvent += OnTabChangedEvent;
                AddTab(actorTab);
            }
        }
        #endregion
    }
}

using System;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Jrpg.Menus
{
    public class ActorTab : Tab
    {
        #region Serialized Fields
        [SerializeField] private Image _icon;
        #endregion

        #region Private Fields
        private RpgActor _actor;
        #endregion

        #region Events
        public event Action<RpgActor> OnTabSelectedEvent = delegate { };
        #endregion

        #region Public Methods
        public void SetActor(RpgActor actor)
        {
            _actor = actor;
            _icon.sprite = _actor.Icon;
        }

        public void OnTabSelected()
        {
            OnTabSelectedEvent(_actor);
        }
        #endregion
    }
}

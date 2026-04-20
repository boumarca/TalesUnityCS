using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus
{
    public class ActorHeader : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image _portrait;
        [SerializeField] private LocalizeStringEvent _actorNameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private LabelBar _hpBar;
        [SerializeField] private LabelBar _mpBar;
        #endregion

        #region Private Fields
        private RpgActor _actor;
        #endregion

        #region Public Methods
        public void Refresh(RpgActor actor)
        {
            _actor = actor;
            _portrait.sprite = _actor.Headshot;
            _actorNameText.StringReference = _actor.Name;
            _levelText.text = _actor.Level.ToString();
            _hpBar.SetBarValue(_actor.CurrentHp, _actor.GetStatValue(RpgStats.MaxHp));
            _mpBar.SetBarValue(_actor.CurrentMp, _actor.GetStatValue(RpgStats.MaxMp));
        }
        #endregion
    }
}

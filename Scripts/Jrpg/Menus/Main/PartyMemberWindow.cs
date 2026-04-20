using Framework.Assertions;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Main
{
    public class PartyMemberWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Color _reserveColor;
        [SerializeField] private Image _background;
        [SerializeField] private Image _actorPortrait;
        [SerializeField] private LocalizeStringEvent _actorName;
        [SerializeField] private TextMeshProUGUI _actorLevel;
        [SerializeField] private TextMeshProUGUI _actorAp;
        [SerializeField] private LabelBar _actorHpBar;
        [SerializeField] private LabelBar _actorMpBar;
        [SerializeField] private ExperienceBar _actorXpBar;
        #endregion

        #region Private Fields
        private RpgActor _actor;
        #endregion

        #region Public Methods
        public void SetActorInfos(RpgActor actor, bool isReserve)
        {
            AssertWrapper.IsNotNull(actor, "Cannot set infos of a null actor");
            _actor = actor;
            _actorPortrait.sprite = _actor.Headshot;
            _actorName.StringReference = _actor.Name;
            _actorLevel.text = _actor.Level.ToString();
            _actorAp.text = _actor.CurrentAp.ToString();
            RefreshBars();            
            if (isReserve)
                _background.color *= _reserveColor;
        }

        public void RefreshBars()
        {
            _actorHpBar.SetBarValue(_actor.CurrentHp, _actor.GetStatValue(RpgStats.MaxHp));
            _actorMpBar.SetBarValue(_actor.CurrentMp, _actor.GetStatValue(RpgStats.MaxMp));
            _actorXpBar.FillExpBar(_actor.LevelInfo);
        }
        #endregion
    }
}

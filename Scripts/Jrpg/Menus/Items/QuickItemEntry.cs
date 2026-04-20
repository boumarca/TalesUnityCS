using System;
using System.Collections.Generic;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Jrpg.Menus.Items
{
    public class QuickItemEntry : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image _portraitBg;
        [SerializeField] private Color _bgBaseColor;
        [SerializeField] private Image _actorPortrait;
        [SerializeField] private GameObject _equippedIcon;
        [SerializeField] private GameObject _statState;
        [SerializeField] private GameObject _barState;
        [SerializeField] private EquipStatEntry[] _equipStatEntries;
        [SerializeField] private LabelBar _hpBar;
        [SerializeField] private LabelBar _mpBar;
        #endregion

        #region Private Fields
        private bool _isUsable;
        private RpgActor _actor;
        private RpgItem _currentItem;
        #endregion

        #region Events
        public event Action<RpgActor> OnActorConfirmedEvent = delegate { };
        #endregion

        #region Public Methods
        public void Initialize(RpgActor actor, RpgItem item)
        {
            _actor = actor;
            _currentItem = item;
            _actorPortrait.sprite = actor.Headshot;
        }

        public void Refresh()
        {
            //TODO: Separate in two classes if it ever grows too big.
            if (_currentItem.IsEquipment)
            {
                SetUsable(_actor.CanEquip(_currentItem));
                if (!_isUsable)
                    return;

                IReadOnlyDictionary<RpgStats, int> projectedStats = ActorManager.Instance.ProjectNewEquipmentStats(_actor, _currentItem.EquippableSlot, _currentItem);
                ShowEquippableState(_currentItem, projectedStats);
            }
            else if (_currentItem.IsConsummable)
            {
                SetUsable(!_actor.IsFullHp);
                ShowHealthBarState(_currentItem);
            }
        }             

        public void Select()
        {
            _hpBar.ShowProjectedIncrease();
        }

        public void Deselect()
        {
            _hpBar.HideProjectedIncrease();
        }
        #endregion

        #region Unity UI Event Methods
        public void UIOnEntryClicked()
        {
            if(_isUsable)
                OnActorConfirmedEvent(_actor);
        }
        #endregion

        #region Private Methods
        private void SetUsable(bool canUse)
        {
            _isUsable = canUse;
            DisplayStatEntries(canUse);
            _actorPortrait.color = canUse ? Color.white : Color.gray7;
            _portraitBg.color = canUse ? _bgBaseColor : _bgBaseColor * 0.7f;
        }

        private void ShowEquippableState(RpgItem item, IReadOnlyDictionary<RpgStats, int> projectedStats)
        {
            _statState.SetActive(true);
            bool isEquipped = _actor.IsEquipped(item);
            _equippedIcon.SetActive(isEquipped);

            int entryIndex = 0;
            foreach (KeyValuePair<RpgStats, int> stat in projectedStats)
            {
                if (isEquipped && item.GetStatValue(stat.Key) == 0
                    || !isEquipped && stat.Value == _actor.GetStatValue(stat.Key))
                    continue;

                EquipStatEntry statEntry = _equipStatEntries[entryIndex];
                statEntry.SetStatType(stat.Key);
                statEntry.SetOldStat(_actor);
                statEntry.SetProjectedStat(_actor, stat.Value);
                entryIndex++;
            }

            for (; entryIndex < _equipStatEntries.Length; entryIndex++)
                _equipStatEntries[entryIndex].gameObject.SetActive(false);
        }

        private void ShowHealthBarState(RpgItem item)
        {
            _barState.SetActive(true);
            _hpBar.SetBarValue(_actor.CurrentHp, _actor.GetStatValue(RpgStats.MaxHp));
            _mpBar.SetBarValue(_actor.CurrentMp, _actor.GetStatValue(RpgStats.MaxMp));
            //TODO: Handle mp and other item effects
            _hpBar.SetProjectedIncrease(_actor.CurrentHp, _actor.GetStatValue(RpgStats.MaxHp), item.EffectValue);
        }

        private void DisplayStatEntries(bool display)
        {
            foreach (EquipStatEntry entry in _equipStatEntries)
                entry.gameObject.SetActive(display);
        }
        #endregion
    }
}

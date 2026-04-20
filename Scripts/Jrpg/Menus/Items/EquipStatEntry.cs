using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace Jrpg.Menus.Items
{
    public class EquipStatEntry : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private RpgStats _stat;
        [SerializeField] private LocalizeStringEvent _statNameText; 
        [SerializeField] private TextMeshProUGUI _oldStatText;
        [SerializeField] private TextMeshProUGUI _newStatText;
        [SerializeField] private Color _neutralStatColor;
        [SerializeField] private Color _increasedStatColor;
        [SerializeField] private Color _decreasedStatColor;
        #endregion

        #region Public Properties
        public RpgStats Stat => _stat;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            SetStatLabel();
        }
        #endregion

        #region Public Methods
        public void SetOldStat(RpgActor actor)
        {
            _oldStatText.text = actor.GetStatValue(_stat).ToString();
        }

        public void SetProjectedStat(RpgActor actor, int projectedValue)
        {
            _newStatText.text = projectedValue.ToString();
            SetStatColor(actor.GetStatValue(_stat), projectedValue);
        }

        public void SetStatType(RpgStats stat)
        {
            _stat = stat;
            SetStatLabel();
        }
        #endregion

        #region Private Methods
        private void SetStatLabel()
        {
            _statNameText.StringReference = new LocalizedString("UI Table", $"$stats.abbr_{(int)_stat}");
        }

        private void SetStatColor(int currentStat, int projectedValue)
        {
            if (currentStat > projectedValue)
                _newStatText.color = _decreasedStatColor;
            else if (currentStat < projectedValue)
                _newStatText.color = _increasedStatColor;
            else
                _newStatText.color = _neutralStatColor;
        }
        #endregion
    }
}

using System.Collections.Generic;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using Jrpg.Menus.Equip;
using Jrpg.Menus.Items;
using SerializedTuples;
using SerializedTuples.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Status
{
    public class StatusMenuWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ElementIconMap _elementIconMap;
        [SerializeField] private LocalizeStringEvent _actorNameText;
        [SerializeField] private Image _actorPortrait;
        [SerializeField] private Image _actorPortraitShadow;
        [Header("Stat page")]
        [SerializeField] private GameObject _statsPage;
        [SerializeField] private TextMeshProUGUI _actorLevel;
        [SerializeField] private TextMeshProUGUI _actorAp;
        [SerializeField] private LabelBar _hpBar;
        [SerializeField] private LabelBar _mpBar;
        [SerializeField] private ExperienceBar _expBar;
        [SerializeField] private TextMeshProUGUI _totalXp;
        [SerializedTupleLabels("Stat", "Text")]
        [SerializeField] private SerializedTuple<RpgStats, TextMeshProUGUI>[] _stats;
        [Header("Equipment page")]
        [SerializeField] private GameObject _equipmentPage;
        [SerializeField] private EquipSlotEntry[] _equipSlots;
        [SerializeField] private Image[] _attackElementsIcons;
        [SerializeField] private DefenseElementInfo[] _defenseElements;
        #endregion

        #region Private Fields
        private RpgActor _actor;
        private bool _isEquipmentPage;
        #endregion

        #region Public Methods
        public void ChangeActor(RpgActor actor)
        {
            _actor = actor;
            DisplayActor();
            DisplayStatsPageData();
            DisplayEquipmentPageData();
        }

        public void ChangePage()
        {
            _isEquipmentPage = !_isEquipmentPage;
            _statsPage.SetActive(!_isEquipmentPage);
            _equipmentPage.SetActive(_isEquipmentPage);
        }
        #endregion

        #region Private Methods
        private void DisplayActor()
        {
            _actorNameText.StringReference = _actor.Name;
            _actorPortrait.sprite = _actor.Portrait;
            _actorPortraitShadow.sprite = _actor.Portrait;
        }

        private void DisplayStatsPageData()
        {
            _actorLevel.text = _actor.Level.ToString();
            _actorAp.text = _actor.CurrentAp.ToString();
            _hpBar.SetBarValue(_actor.CurrentHp, _actor.GetStatValue(RpgStats.MaxHp));
            _mpBar.SetBarValue(_actor.CurrentMp, _actor.GetStatValue(RpgStats.MaxMp));
            _expBar.FillExpBar(_actor.LevelInfo);
            _totalXp.text = _actor.LevelInfo.TotalExperience.ToString();
            DisplayStats();
        }

        private void DisplayEquipmentPageData()
        {
            DisplayEquipSlots();
            DisplayAttackElements();
            DisplayDefenseElements();
        }

        private void DisplayStats()
        {
            foreach (SerializedTuple<RpgStats, TextMeshProUGUI> tuple in _stats)
                tuple.v2.text = _actor.GetStatValue(tuple.v1).ToString();
        }

        private void DisplayEquipSlots()
        {
            foreach (EquipSlotEntry slot in _equipSlots)
                slot.ChangeItem(_actor.GetEquippedItem(slot.SlotType));
        }

        private void DisplayAttackElements()
        {
            IReadOnlyCollection<RpgElements> elements = _actor.GatherAttackElements();
            int index = 0;
            foreach (RpgElements element in elements)
            {
                Image icon = _attackElementsIcons[index];
                icon.gameObject.SetActive(true);
                icon.sprite = _elementIconMap.GetSprite(element);
                index++;
            }

            for (; index < _attackElementsIcons.Length; index++)
                _attackElementsIcons[index].gameObject.SetActive(false);
        }

        private void DisplayDefenseElements()
        {
            IReadOnlyDictionary<RpgElements, int> elements = _actor.GatherDefenseElements();
            int index = 0;
            foreach (KeyValuePair<RpgElements, int> element in elements)
            {
                DefenseElementInfo defenseElementInfo = _defenseElements[index];
                defenseElementInfo.SetInfos(_elementIconMap.GetSprite(element.Key), element.Value);
                defenseElementInfo.gameObject.SetActive(true);
                index++;
            }

            for (; index < _defenseElements.Length; index++)
                _defenseElements[index].gameObject.SetActive(false);
        }
        #endregion
    }
}

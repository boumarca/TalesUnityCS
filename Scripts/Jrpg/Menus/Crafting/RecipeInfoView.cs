using System.Collections.Generic;
using System.Globalization;
using Game.CraftingSystem.Data;
using Game.RpgSystem;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using SerializedTuples;
using SerializedTuples.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class RecipeInfoView : InfoView<CraftingRecipeData>
    {
        #region Serialized Fields
        [SerializeField] private ElementIconMap _elementIconMap;
        [SerializeField] private LocalizeStringEvent _itemNameText;
        [SerializeField] private LocalizeStringEvent _itemDescriptionText;
        [SerializeField] private Image _itemImage;
        [SerializeField] private LocalizeStringEvent[] _itemCategoriesTexts;
        [SerializedTupleLabels("Stat", "StatText")]
        [SerializeField] private SerializedTuple<RpgStats, TextMeshProUGUI>[] _statsInfos;
        [SerializeField] private Image[] _attackElementIcons;
        [SerializeField] private Image[] _defenseElementIcons;
        [SerializeField] private Image[] _partyMembersIcons;
        #endregion

        #region InfoView Implementation
        protected override void DisplayInfos()
        {
            DisplayItemImage();
            DisplayItemInfos();
            DisplayItemCategories();
            if (SelectedInfo.Item.IsEquipment)
            {
                DisplayEquipmentStats();
                DisplayEquippableMembers();
            }
        }

        protected override void ClearInfos()
        {
            _itemImage.gameObject.SetActive(false);
        }

        protected override bool IsPageValid()
        {
            return SelectedInfo.Item.IsKeyItem || (!SelectedInfo.Item.IsEquipment && CurrentPage > 0);
        }

        #endregion

        #region Private Methods
        private void DisplayItemImage()
        {
            _itemImage.sprite = SelectedInfo.Item.InfoImage;
            _itemImage.gameObject.SetActive(true);
        }

        private void DisplayItemInfos()
        {
            _itemNameText.StringReference = SelectedInfo.Item.Name;
            _itemDescriptionText.StringReference = SelectedInfo.Item.Description;
        }

        private void DisplayItemCategories()
        {
            int textIndex = 0;
            foreach(ItemCategory category in SelectedInfo.Item.Categories)
            {
                LocalizeStringEvent itemCategoryText = _itemCategoriesTexts[textIndex];
                itemCategoryText.StringReference = category.ToLocalizedString();
                itemCategoryText.gameObject.SetActive(true);
                textIndex++;
            }

            for (; textIndex < _itemCategoriesTexts.Length; textIndex++)
            {
                _itemCategoriesTexts[textIndex].gameObject.SetActive(false);
            }
        }

        private void DisplayEquipmentStats()
        {
            foreach(SerializedTuple<RpgStats, TextMeshProUGUI> stat in _statsInfos)
                stat.v2.text = SelectedInfo.Item.GetStatValue(stat.v1).ToString(CultureInfo.InvariantCulture);

            DisplayAttackElements();
            DisplayDefenseElements();
        }

        private void DisplayAttackElements()
        {
            int index = 0;
            if(SelectedInfo.Item.AttackElements != null)
            {
                foreach (RpgElements element in SelectedInfo.Item.AttackElements)
                {
                    Image icon = _attackElementIcons[index];
                    icon.gameObject.SetActive(true);
                    icon.sprite = _elementIconMap.GetSprite(element);
                    index++;
                }
            }

            for (; index < _attackElementIcons.Length; index++)
                _attackElementIcons[index].gameObject.SetActive(false);
        }

        private void DisplayDefenseElements()
        {
            int index = 0;
            if(SelectedInfo.Item.DefenseElements != null)
            {
                foreach (KeyValuePair<RpgElements, int> element in SelectedInfo.Item.DefenseElements)
                {
                    Image icon = _defenseElementIcons[index];
                    icon.gameObject.SetActive(true);
                    icon.sprite = _elementIconMap.GetSprite(element.Key);
                    index++;
                }
            }

            for (; index < _defenseElementIcons.Length; index++)
                _defenseElementIcons[index].gameObject.SetActive(false);
        }

        private void DisplayEquippableMembers()
        {
            int iconIndex = 0;
            foreach (RpgActor actor in PartyManager.Instance.CurrentParty.Members)
            {
                Image icon = _partyMembersIcons[iconIndex];
                icon.gameObject.SetActive(true);
                icon.sprite = actor.Chara;
                icon.color = actor.CanEquip(SelectedInfo.Item) ? Color.white : Color.gray3;
                iconIndex++;
            }

            for (; iconIndex < _partyMembersIcons.Length; iconIndex++)
                _partyMembersIcons[iconIndex].gameObject.SetActive(false);
        }
        #endregion
    }
}

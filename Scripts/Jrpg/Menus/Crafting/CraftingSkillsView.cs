using System.Collections.Generic;
using System.Globalization;
using Game.CraftingSystem;
using Game.CraftingSystem.Data;
using Game.RpgSystem.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Jrpg.Menus.Crafting
{
    public class CraftingSkillsView : MonoBehaviour
    {
        [SerializeField] private ActorHeader _actorHeader;
        [SerializeField] private TextMeshProUGUI _craftingLevel;
        [SerializeField] private LocalizeStringEvent[] _craftingSkillsTexts;

        public void DisplayCraftingLevel(int level)
        {
            _craftingLevel.text = level.ToString(CultureInfo.InvariantCulture);
        }

        public void ChangeActor(RpgActor actor)
        {
            _actorHeader.Refresh(actor);
            DisplayCraftingSkills(CraftingManager.Instance.GetOrderedActorCraftingSkills(actor.Id));
        }

        private void DisplayCraftingSkills(IReadOnlyCollection<CraftingSkillData> craftingSkills)
        {
            int i = 0;
            foreach (CraftingSkillData skillData in craftingSkills)
            {
                //if (!IsSkillActive(skillData))
                 //   continue;

                DisplaySkill(skillData, _craftingSkillsTexts[i]);
                i++;
            }

            for (; i < _craftingSkillsTexts.Length; i++)
                _craftingSkillsTexts[i].gameObject.SetActive(false);
        }

        private void DisplaySkill(CraftingSkillData skillData, LocalizeStringEvent localizer)
        {
            localizer.StringReference = skillData.Name;
            localizer.gameObject.SetActive(true);

            //Disable
            TextMeshProUGUI textComponent = localizer.GetComponent<TextMeshProUGUI>();
            textComponent.color = IsSkillActive(skillData) ? Color.white : Color.gray5;
        }

        private bool IsSkillActive(CraftingSkillData skillData)
        {
            if (skillData.AddedEffect == null)
                return true;

            return skillData.AddedEffect.HasCompatibleType(CraftingManager.Instance.CraftingItemModel.Type);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Game.CraftingSystem.Data;
using Game.CraftingSystem.Models;
using Game.RpgSystem.Data;
using Game.RpgSystem.Models;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class CraftingResultWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private LocalizeStringEvent _titleText;
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _craftingLevelText;
        [SerializeField] private LabelBar _xpBar;
        [SerializeField] private TextMeshProUGUI _xpGainText;
        [SerializeField] private GameObject _levelUpText;
        [SerializeField] private GameObject _craftingBonusSection;
        [SerializeField] private TextMeshProUGUI[] _craftingBonusTexts;
        #endregion

        #region Private Fields
        private int _previousLevel;
        private int _newLevel;
        private int _previousXp;
        private int _newXp;
        private float _previousBarProgress;
        private float _newBarProgress;
        private int _gainedXp;
        #endregion

        #region Events
        public event Action OnCloseEvent = delegate { };
        #endregion

        #region Public Methods
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            StartCoroutine(CloseCoroutine());
        }

        public void SetItem(CraftingRecipeData recipe)
        {
            RpgItemData item = recipe.Item;
            _titleText.StringReference.Arguments = new List<object> { item.Name.GetLocalizedString() }; //TODO: Avoid new
            _titleText.RefreshString();
            _itemImage.sprite = item.InfoImage;
        }

        public void SetPreviousXpValues(LevelInfo levelInfo)
        {
            _previousLevel = levelInfo.Level;
            _previousXp = levelInfo.RemainingExperienceToNextLevel();
            _previousBarProgress = levelInfo.GetProgressPercentToNextLevel();
        }

        public void SetGainedXp(int xp)
        {
            _gainedXp = xp;            
        }

        public void SetTargetXpValues(LevelInfo levelInfo)
        {
            _newLevel = levelInfo.Level;
            _newXp = levelInfo.RemainingExperienceToNextLevel();
            _newBarProgress = levelInfo.GetProgressPercentToNextLevel();
        }

        public void SetCraftingBonus(IReadOnlyList<CraftingSkillResult> bonusCollection)
        {
            _craftingBonusSection.SetActive(bonusCollection.Count > 0);
            for (int i = 0; i < _craftingBonusTexts.Length; i++)
            {
                TextMeshProUGUI bonusText = _craftingBonusTexts[i];
                if (i >= bonusCollection.Count)
                {
                    bonusText.gameObject.SetActive(false);
                    continue;
                }

                bonusText.gameObject.SetActive(true);
                bonusText.text = bonusCollection[i].ResultMessage;
            }
        }

        public void StartXpAnimation()
        {
            StartCoroutine(XpAnimationCoroutine());
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnConfirmPerformed()
        {
            Hide();
        }
        #endregion

        #region Private Methods
        private IEnumerator XpAnimationCoroutine()
        {
            _levelUpText.SetActive(false);
            _craftingLevelText.text = _previousLevel.ToString(CultureInfo.InvariantCulture);
            _xpBar.SetBarValue(_previousXp.ToString(CultureInfo.InvariantCulture), _previousBarProgress);
            _xpGainText.text = $" +{_gainedXp}";

            yield return new WaitForSeconds(1);

            _craftingLevelText.text = _newLevel.ToString(CultureInfo.InvariantCulture);
            _xpBar.SetBarValue(_newXp.ToString(CultureInfo.InvariantCulture), _newBarProgress);
            _xpGainText.text = $" +0";
            _levelUpText.SetActive(_newLevel > _previousLevel);
        }

        private IEnumerator CloseCoroutine()
        {
            yield return null;
            OnCloseEvent();
            gameObject.SetActive(false);
        }
        #endregion
    }
}

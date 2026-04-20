using System;
using System.Collections.Generic;
using System.Linq;
using Game.CraftingSystem;
using Game.RpgSystem.Models;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Jrpg.Menus.Crafting
{
    public class EffectListWindow : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private InputtableWindow _window;
        [SerializeField] private ListView _listView;
        [SerializeField] private Button _confirmButton;
        #endregion

        #region Public Properties
        public InputtableWindow Window => _window;
        public EffectListEntry SelectedEntry => _listView.SelectedListEntry as EffectListEntry;
        public bool IsCraftButtonSelected { get; private set; }
        #endregion

        #region Events
        public event Action<EffectListEntry> OnSelectedEffectChangedEvent = delegate { };
        public event Action<object> OnSelectedEffectConfirmedEvent = delegate { };
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _window.OnActivatedEvent += HandleOnActivated;
            _listView.OnSelectedEntryChangedEvent += HandleOnSelectedEntryChanged;
        }

        private void OnDisable()
        {
            _window.OnActivatedEvent -= HandleOnActivated;
            _listView.OnSelectedEntryChangedEvent -= HandleOnSelectedEntryChanged;
        }
        #endregion

        #region Public Methods
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void RefreshEffectList(IReadOnlyCollection<ItemEffect> effects, bool allowSelection = false)
        {
            _listView.PopulateList(effects, OnSelectedEffectConfirmedEvent);
            SetConfirmButtonNavigation();
            RefreshEntries(allowSelection);
        }

        public void SelectNextEffect()
        {
            _listView.SelectNext(entry => CanSelect(entry), _confirmButton.gameObject);

            bool CanSelect(ListEntry entry)
            {
                if (CraftingManager.Instance.CraftingItemModel.SelectedEffects.Count >= RpgItem.MaxEffectCount)
                    return false;

                return !((EffectListEntry)entry).IsChecked;
            }
        }

        public void BackToLastEffect()
        {
            _listView.SelectLastValidEntry();
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnSelectCraftButton()
        {
            IsCraftButtonSelected = true;
        }

        public void UIOnDeselectCraftButton()
        {
            IsCraftButtonSelected = false;
        }
        #endregion

        #region Private Methods
        private void SetConfirmButtonNavigation()
        {
            Navigation navigation = _confirmButton.navigation;
            navigation.selectOnUp = _listView.Last.GetComponent<Selectable>();
            _confirmButton.navigation = navigation;
        }

        private void RefreshEntries(bool allowSelection)
        {
            foreach (ListEntry entry in _listView.Entries)
            {
                EffectListEntry effectEntry = entry as EffectListEntry;
                ItemEffect effect = effectEntry.Data as ItemEffect;
                effectEntry.SetLeveledUp(CraftingManager.Instance.CraftingItemModel.IsNewEffect(effect));
                if (!allowSelection)
                    continue;
                effectEntry.SetChecked(CraftingManager.Instance.CraftingItemModel.SelectedEffects.Contains(effect));
            }
        }

        private void HandleOnActivated()
        {
            _listView.SelectIndex(0);
        }

        private void HandleOnSelectedEntryChanged(ListEntry entry)
        {
            OnSelectedEffectChangedEvent(entry as EffectListEntry);
        }
        #endregion
    }
}

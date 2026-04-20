using System.Threading.Tasks;
using Framework.Extensions;
using Game.SaveSystem;
using Jrpg.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

namespace Jrpg.Menus.File
{
    public class SaveMenuStateBehaviour : FileMenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private SaveSlotWindow _newSlot;
        [SerializeField] private LocalizedString _overwriteMessageKey;
        [SerializeField] private LocalizedString _newSlotMessageKey;
        #endregion

        #region GameStateBehaviour Implementation
        public override async Task OnEnterState(object payload)
        {
            await base.OnEnterState(payload);
            if (SuspendSaveWindow != null)
                SuspendSaveWindow.gameObject.SetActive(false);
            if(AutoSaveWindow != null)
                AutoSaveWindow.gameObject.SetActive(false);

            InsertSlot(0, _newSlot);
            RefreshChangePageInput();
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnNewSlotClicked()
        {
            OnNewSlotClicked();
        }
        #endregion

        #region Override Methods

        protected override void OnSlotClicked(SaveSlotWindow slotWindow)
        {
            base.OnSlotClicked(slotWindow);
            DisplayConfirmationPopup(_overwriteMessageKey, OnConfirm);
            return;

            void OnConfirm()
            {
                OverwriteSlot(slotWindow);
            }
        }

        protected override GameObject FirstSlot()
        {
            return _newSlot.gameObject;
        }

        protected override bool CanDeleteSlot(SaveSlotWindow slot)
        {
            return base.CanDeleteSlot(slot) && slot != _newSlot;
        }
        #endregion

        #region Private Methods
        private void OnNewSlotClicked()
        {
            DisplayConfirmationPopup(_newSlotMessageKey, OnConfirm);
            return;

            void OnConfirm()
            {
                CreateNewSlot().FireAndForget();
            }
        }

        private async Task CreateNewSlot()
        {
            int newIndex = SaveDataManager.Instance.CreateNewSaveGame();
            SaveMetadataBase metadata = SaveDataManager.Instance.GetMetadataForIndex(newIndex);
            SaveSlotWindow saveSlot =  await CreateSlot(metadata);
            if (saveSlot == null)
                return;

            saveSlot.transform.SetSiblingIndex(1);
            AddNewSlot(saveSlot);
            EventSystem.current.SetSelectedGameObject(_newSlot.gameObject);
            RefreshChangePageInput();
        }

        private void OverwriteSlot(SaveSlotWindow slotWindow)
        {
            int slotIndex = slotWindow.SlotIndex;
            SaveDataManager.Instance.SaveGameAtIndex(slotIndex);
            SaveMetadataBase metadata = SaveDataManager.Instance.GetMetadataForIndex(slotIndex);
            if (metadata is not RpgSaveMetadata rpgMetadata)
                return;

            slotWindow.PopulateWindow(rpgMetadata);
            EventSystem.current.SetSelectedGameObject(slotWindow.gameObject);
        }
        #endregion
    }
}

using System.Threading.Tasks;
using Game.SaveSystem;
using Game.StateStack;
using UnityEngine;
using UnityEngine.Localization;

namespace Jrpg.Menus.File
{
    public class LoadMenuStateBehaviour : FileMenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private LocalizedString _loadMessageKey;
        [SerializeField] private LocalizedString _loadQuickSaveKey;
        #endregion

        #region GameStateBehaviour Implementation
        public override async Task OnEnterState(object payload)
        {
            await base.OnEnterState(payload);
            if (HasAutoSave)
            {
                InsertSlot(0, AutoSaveWindow);
                AutoSaveWindow.transform.SetAsFirstSibling();
            }
            if (HasSuspendedSave)
            {
                InsertSlot(0, SuspendSaveWindow);
                SuspendSaveWindow.transform.SetAsFirstSibling();
            }
            RefreshChangePageInput();
        }
        #endregion

        #region Override Methods
        protected override void OnSlotClicked(SaveSlotWindow slotWindow)
        {
            base.OnSlotClicked(slotWindow);
            bool isSuspendSave = slotWindow == SuspendSaveWindow;
            DisplayConfirmationPopup(isSuspendSave ? _loadQuickSaveKey : _loadMessageKey, OnConfirm, selectConfirm: true);
            return;

            void OnConfirm()
            {
                LoadSlot(slotWindow.SlotIndex);
                if (isSuspendSave)
                    SaveDataManager.Instance.DeleteGameAtIndex(slotWindow.SlotIndex);
            }
        }

        protected override GameObject FirstSlot()
        {
            if (HasSuspendedSave)
                return SuspendSaveWindow.gameObject;
            if (HasAutoSave)
                return AutoSaveWindow.gameObject;
            return base.FirstSlot();
        }
        #endregion

        #region Private Methods
        private void LoadSlot(int slotId)
        {
            StateStackManager.Instance.ClearStack();
            SaveDataManager.Instance.LoadGameAtIndex(slotId);
        }
        #endregion
    }
}

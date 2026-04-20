using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Inputs;
using Game.SaveSystem;
using Game.UI;
using Jrpg.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace Jrpg.Menus.File
{
    public class FileMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private SaveSlotWindow _saveSlotPrefab;
        [SerializeField] private Transform _saveSlotContainer;
        [SerializeField] private GameObject _slotView;
        [SerializeField] private LocalizedString _deleteSaveMessageKey;
        [SerializeField] private InputtableWindow _fileSlotView;
        [SerializeField] private ListView _listView;
        #endregion

        #region Private Fields
        private SaveSlotWindow _suspendSaveWindow;
        private SaveSlotWindow _autoSaveWindow;
        #endregion

        #region Protected Properties
        protected SaveSlotWindow SuspendSaveWindow => _suspendSaveWindow;
        protected SaveSlotWindow AutoSaveWindow => _autoSaveWindow;
        protected bool HasSuspendedSave => _suspendSaveWindow != null;
        protected bool HasAutoSave => _autoSaveWindow != null;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _listView.OnSelectedEntryChangedEvent += HandleSelectedItemChanged;
            _fileSlotView.OnActivatedEvent += RefreshInputs;
        }              

        private void OnDisable()
        {
            _listView.OnSelectedEntryChangedEvent -= HandleSelectedItemChanged;
            _fileSlotView.OnActivatedEvent -= RefreshInputs;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override async Task OnEnterState(object payload)
        {
            await base.OnEnterState(payload);
            await CreateAllSlots();
            _slotView.SetActive(true);
            EventSystem.current.SetSelectedGameObject(FirstSlot());
        }
        #endregion

        #region Virtual Methods
        protected virtual void OnSlotClicked(SaveSlotWindow slotWindow)
        {
        }

        protected virtual GameObject FirstSlot()
        {
            return _saveSlotContainer.childCount > 0 ? _saveSlotContainer.GetChild(0).gameObject : DefaultSelection;
        }

        protected virtual bool CanDeleteSlot(SaveSlotWindow slot)
        {
            return _listView.EntryCount > 1 && slot != _suspendSaveWindow && slot != _autoSaveWindow;
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnDeleteSlotPerformed()
        {
            OnDeleteSlotPerformed();
        }

        public void UIOnChangePagePerformed(InputAction.CallbackContext context)
        {
            float readValue = context.ReadValue<float>();
            //Performed should not return 0;
            _listView.ChangePage(readValue > 0 ? 1 : -1);
        }
        #endregion

        #region Protected Methods
        protected async Task<SaveSlotWindow> CreateSlot(SaveMetadataBase metadata)
        {
            if (metadata is not RpgSaveMetadata rpgMetadata)
                return null;

            AsyncInstantiateOperation<SaveSlotWindow> handle = InstantiateAsync(_saveSlotPrefab, _saveSlotContainer);
            while (!handle.isDone)
                await Task.Yield();
            SaveSlotWindow saveSlot = handle.Result[0];
            saveSlot.PopulateWindow(rpgMetadata);
            saveSlot.OnSlotClickedEvent += OnSlotClicked;
            return saveSlot;
        }

        protected void AddNewSlot(SaveSlotWindow slot)
        {
            InsertSlot(1, slot);
        }

        protected void InsertSlot(int index, SaveSlotWindow slot)
        {
            _listView.InsertEntry(index, slot);
        }

        protected void RefreshDeleteSlotInput()
        {
            bool enableCondition = CanDeleteSlot((SaveSlotWindow)_listView.SelectedListEntry);
            RefreshInput(InputManager.Instance.InputActions.FileMenu.DeleteSlot, enableCondition);
        }

        protected void RefreshChangePageInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangePage, _listView.EntryCount > 1);
        }
        #endregion

        #region Private Methods
        private async Task CreateAllSlots()
        {
            IReadOnlyList<SaveMetadataBase> metadataList = SaveDataManager.Instance.GetAllMetadataSaveFiles();
            for (int i = metadataList.Count - 1; i >= 0; i--)
            {
                SaveSlotWindow slot = await CreateSlot(metadataList[i]);
                if (slot.IsSuspendSave)
                    _suspendSaveWindow = slot;
                else if(slot.IsAutoSave)
                    _autoSaveWindow = slot;
                else
                    _listView.AddEntry(slot);
            }
        }

        private void HandleSelectedItemChanged(ListEntry entry)
        {
            RefreshDeleteSlotInput();
        }

        private void OnDeleteSlotPerformed()
        {
            SaveSlotWindow saveSlot = _listView.SelectedListEntry as SaveSlotWindow;
            if (saveSlot.IsSuspendSave || saveSlot.IsAutoSave)
                return;

            DisplayConfirmationPopup(_deleteSaveMessageKey, OnConfirm);
            return;

            void OnConfirm()
            {
                DeleteSlot(saveSlot);
            }
        }

        private void DeleteSlot(SaveSlotWindow saveSlot)
        {
            //TODO: Use a better way to prevent save menu from being empty
            if (_listView.EntryCount <= 1)
                return;

            bool success = SaveDataManager.Instance.DeleteGameAtIndex(saveSlot.SlotIndex);
            if (!success)
                return;

            _listView.RemoveSelectedEntry();
        }

        private void RefreshInputs()
        {
            RefreshDeleteSlotInput();
            RefreshChangePageInput();
        }
        #endregion
    }
}

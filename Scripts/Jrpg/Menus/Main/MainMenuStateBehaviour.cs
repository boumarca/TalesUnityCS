using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Extensions;
using Framework.Inputs;
using Game.Cameras;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using Game.SaveSystem;
using Game.StateStack;
using Game.UI;
using Jrpg.Maps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Jrpg.Menus.Main
{
    public class MainMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private InputtableWindow _submenuWindow;
        [SerializeField] private PartyMemberWindow _partyMemberWindowPrefab;
        [SerializeField] private Transform _partyWindowsSection;
        [SerializeField] private InfosWindow _infosWindow;
        [SerializeField] private ScrollRect _partyScrollRect;
        [SerializeField] private InputtableWindow _dataSubwindow;
        [SerializeField] private GameObject _dataFirstSelection;
        [SerializeField] private InputtableWindow _systemSubwindow;
        [SerializeField] private GameObject _saveMenuButton;
        [SerializeField] private GameObject _suspendMenuButton;

        [Header("Localization")]
        [SerializeField] private LocalizedString _suspendConfirmationKey;
        [SerializeField] private LocalizedString _quitConfirmationKey;

        [Header("Submenu Buttons")]
        [SerializeField] private Button _loadMenuButton;

        [Header("States")]
        [SerializeField] private StateData _itemsMenuStateData;
        [SerializeField] private StateData _equipMenuStateData;
        [SerializeField] private StateData _craftingMenuStateData;
        [SerializeField] private StateData _statusMenuStateData;
        [SerializeField] private StateData _questMenuStateData;
        [SerializeField] private StateData _saveMenuStateData;
        [SerializeField] private StateData _loadMenuStateData;
        [SerializeField] private StateData _titlescreenStateData;
        #endregion

        #region Private Fields
        private readonly List<PartyMemberWindow> _partyMemberWindows = new();
        private InputtableWindow _currentSubwindow;
        private GameObject _previousSelection;
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            CreatePartyWindows();
            _infosWindow.SetInfos();
            HandlePartyHintVisibility(true);
            return Task.CompletedTask;
        }

        public override void OnResumeState()
        {
            base.OnResumeState();
            foreach (PartyMemberWindow window in _partyMemberWindows)
                window.RefreshBars();

            _infosWindow.SetInfos();
            HandlePartyHintVisibility(true);
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnItemsMenuButtonClicked()
        {
            OpenSubMenu(_itemsMenuStateData);
        }

        public void UIOnEquipMenuButtonClicked()
        {
            OpenSubMenu(_equipMenuStateData);
        }

        public void UIOnCraftingMenuButtonClicked()
        {
            OpenSubMenu(_craftingMenuStateData);
        }

        public void UIOnStatusMenuButtonClicked()
        {
            OpenSubMenu(_statusMenuStateData); 
        }

        public void UIOnQuestMenuButtonClicked()
        {
            OpenSubMenu(_questMenuStateData);
        }

        public void UIOnDataMenuButtonClicked()
        {
            DisplayDataSubwindow();
        }

        public void UIOnSystemMenuButtonClicked()
        {
            DisplaySystemSubwindow();
        }

        public void UIOnSaveMenuButtonClicked()
        {
            OpenSaveMenu();
        }

        public void UIOnSuspendMenuButtonClicked()
        {
            SuspendSaveConfirmation();
        }

        public void UIOnLoadMenuButtonClicked()
        {
            OpenSubMenu(_loadMenuStateData);
        }

        public void UIOnQuitMenuButtonClicked()
        {
            QuitGameConfirmation();
        }

        public void UIOnSubwindowCancelPerformed()
        {
            CloseSubwindow();
        }

        public void UIOnShowActiveMembersPerformed()
        {
            ShowActiveParty();
        }

        public void UIOnShowReserveMembersPerformed()
        {
            ShowReserveParty();
        }
        #endregion

        #region Private Methods
        private void CreatePartyWindows()
        {
            RpgParty party = PartyManager.Instance.CurrentParty;
            for (int i = 0; i < party.Members.Count; i++)
            {
                RpgActor actor = party.Members[i];
                PartyMemberWindow window = Instantiate(_partyMemberWindowPrefab, _partyWindowsSection);
                window.SetActorInfos(actor, i >= RpgParty.MaxActivePartySize);
                _partyMemberWindows.Add(window);
            }
        }

        //TODO: Review and simplify
        private void HandlePartyHintVisibility(bool show)
        {
            InputManager inputManager = InputManager.Instance;
            GameInputs.MainMenuActions actions = InputManager.Instance.InputActions.MainMenu;
            if (!show)
            {
                inputManager.DisableInput(actions.ActiveMembers);
                inputManager.DisableInput(actions.ReserveMembers);
                return;
            }

            if (_partyScrollRect.horizontalNormalizedPosition == 0)
            {
                inputManager.DisableInput(actions.ActiveMembers);
                if (PartyManager.Instance.CurrentParty.HasReserve)
                    inputManager.EnableInput(actions.ReserveMembers);
                else
                    inputManager.DisableInput(actions.ReserveMembers);
            }
            else
            {
                inputManager.EnableInput(actions.ActiveMembers);
                inputManager.DisableInput(actions.ReserveMembers);
            }
        }

        private void ShowActiveParty()
        {
            _partyScrollRect.horizontalNormalizedPosition = 0;
            HandlePartyHintVisibility(true);
        }

        private void ShowReserveParty()
        {
            if (!PartyManager.Instance.CurrentParty.HasReserve)
                return;

            _partyScrollRect.horizontalNormalizedPosition = 1;
            HandlePartyHintVisibility(true);
        }

        private void OpenSubMenu(StateData state)
        {
            StateStackManager.Instance.PushState(state);
        }

        private void DisplayDataSubwindow()
        {
            DisplaySubwindow(_dataSubwindow, _dataFirstSelection);
        }

        private void DisplaySystemSubwindow()
        {
            bool isSaveAllowed = IsSaveAllowed();
            _saveMenuButton.SetActive(isSaveAllowed);
            _suspendMenuButton.SetActive(!isSaveAllowed);
            _loadMenuButton.interactable = SaveDataManager.Instance.HasSaveFile();
            DisplaySubwindow(_systemSubwindow, isSaveAllowed ? _saveMenuButton : _suspendMenuButton);
        }

        private void OpenSaveMenu()
        {
            if (IsSaveAllowed())
                OpenSubMenu(_saveMenuStateData);
        }

        private void SuspendSaveConfirmation()
        {
            DisplayConfirmationPopup(_suspendConfirmationKey, OnConfirm);
            return;

            void OnConfirm()
            {
                SaveDataManager.Instance.SaveGameAtIndex(0);
                QuitGame();
            }
        }

        private void QuitGameConfirmation()
        {
            DisplayConfirmationPopup(_quitConfirmationKey, OnConfirm);
            return;

            void OnConfirm()
            {
                QuitGame();
            }
        }

        private void QuitGame()
        {
            QuitGameAsync().FireAndForget();
        }

        private async Task QuitGameAsync()
        {
            //TODO: Review inputs disabling.
            InputManager.Instance.DisableCurrentInputMap();
            _submenuWindow.Deactivate();
            await ScreenCamera.Instance.FadeOut();
            StateStackManager.Instance.ClearStack();
            await StateStackManager.Instance.ChangeToStateAsync(_titlescreenStateData);
        }

        private void CloseSubwindow()
        {
            ChangeWindowFocus(_submenuWindow);
            _currentSubwindow.gameObject.SetActive(false);
            _currentSubwindow = null;
            EventSystem.current.SetSelectedGameObject(_previousSelection);
            HandlePartyHintVisibility(true);
        }

        private void DisplaySubwindow(InputtableWindow subwindow, GameObject selectedObject)
        {
            EventSystem eventSystem = EventSystem.current;
            _previousSelection = eventSystem.currentSelectedGameObject;
            _currentSubwindow = subwindow;
            _currentSubwindow.gameObject.SetActive(true);
            ChangeWindowFocus(_currentSubwindow);
            eventSystem.SetSelectedGameObject(selectedObject);
            HandlePartyHintVisibility(false);
        }

        private static bool IsSaveAllowed()
        {
            return MapStateManager.Instance.AllowSave;
        }
        #endregion
    }
}

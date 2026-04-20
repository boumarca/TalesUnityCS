using System.Threading.Tasks;
using Framework.Inputs;
using Game.RpgSystem.Models;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jrpg.Menus.Status
{
    public class StatusMenuStateBehaviour : MenuStateBehaviour
    {
        #region Serialized Fields
        [SerializeField] private StatusMenuWindow _statusWindow;
        [SerializeField] private PartyMembersTabView _partyMembersTabView;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _partyMembersTabView.OnTabChangedEvent += HandleTabChanged;
        }

        private void OnDisable()
        {
            _partyMembersTabView.OnTabChangedEvent -= HandleTabChanged;
        }
        #endregion

        #region GameStateBehaviour Implementation
        public override Task OnEnterState(object payload)
        {
            base.OnEnterState(payload);
            _partyMembersTabView.PopulateTabs();
            _partyMembersTabView.SelectFirstTab();
            DisableConfirmInput();
            RefreshChangeTabInput();
            return Task.CompletedTask;
        }
        #endregion

        #region Unity UI Event Methods
        public void UIOnChangeTabPerformed(InputAction.CallbackContext context)
        {
            _partyMembersTabView.ChangeTab((int)context.ReadValue<float>());
        }

        public void UIOnChangeInfoPerformed()
        {
            _statusWindow.ChangePage();
        }
        #endregion

        #region Private Methods
        private void HandleTabChanged(RpgActor actor)
        {
            _statusWindow.ChangeActor(actor);
        }

        private void RefreshChangeTabInput()
        {
            RefreshInput(InputManager.Instance.InputActions.MenuCommon.ChangeTab, _partyMembersTabView.Count > 1);
        }
        #endregion
    }
}

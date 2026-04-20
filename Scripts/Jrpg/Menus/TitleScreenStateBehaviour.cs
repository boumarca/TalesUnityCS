using System.Threading.Tasks;
using Game.Cameras;
using Game.CraftingSystem;
using Game.QuestSystem;
using Game.RpgSystem;
using Game.SaveSystem;
using Game.StateStack;
using Game.Stats;
using Game.UI;
using Jrpg.Maps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jrpg.Menus
{
    public class TitleScreenStateBehaviour : GameStateBehaviour
    {
        #region Serialized Field
        [Header("Component references")]
        [SerializeField] private InputtableWindow _window;
        [SerializeField] private GameObject _defaultSelection;
        [SerializeField] private Selectable _loadGameButton;

        [Header("States")]
        [SerializeField] private StateData _loadMenuState;
        #endregion

        #region Private Fields
        private GameObject _lastSelection;
        #endregion

        #region GameStateBehaviour Implementation
        public override async Task OnEnterState(object payload)
        {
            await ScreenCamera.Instance.FadeIn();
            ActorManager.Instance.InitializeAsNew(); //Necessary to have access to the actors for the load menu.
            _loadGameButton.interactable = SaveDataManager.Instance.HasSaveFile();
            _window.Activate();
            EventSystem.current.SetSelectedGameObject(_defaultSelection);
            return;
        }

        public override void OnExitState()
        {
        }

        public override void OnSuspendState()
        {
            _lastSelection = EventSystem.current.currentSelectedGameObject;
            _window.Deactivate();
            gameObject.SetActive(false);
        }

        public override void OnResumeState()
        {
            gameObject.SetActive(true);
            _window.Activate();
            EventSystem.current.SetSelectedGameObject(_lastSelection);
        }
        #endregion

        #region Unity Event UI Methods
        public void UIOnNewGameButtonClicked()
        {
            StartNewGame();
        }

        public void UIOnLoadGameButtonClicked()
        {
            LoadGame();
        }

        public void UIOnQuitButtonClicked()
        {
            QuitGame();
        }
        #endregion

        #region Private Methods
        private void StartNewGame()
        {
            GameStatsManager.Instance.InitializeAsNew();
            InventoryManager.Instance.InitializeAsNew();
            PartyManager.Instance.InitializeAsNew();
            QuestManager.Instance.InitializeAsNew();
            CraftingManager.Instance.InitializeAsNew();
            MapStateManager.Instance.LoadDefaultMap();
        }

        private void LoadGame()
        {
            StateStackManager.Instance.PushState(_loadMenuState);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}

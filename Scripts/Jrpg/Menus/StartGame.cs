using Framework.Extensions;
using Game.SaveSystem;
using Game.StateStack;
using UnityEngine;

namespace Jrpg.Menus
{
    public class StartGame : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private StateData _titleScreenState;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            InitializeSystems();
            LoadTitleScreen();
        }
        #endregion

        #region Private Methods
        private void InitializeSystems()
        {
            SaveDataManager.Instance.Initialize();
        }

        private void LoadTitleScreen()
        {
            StateStackManager.Instance.InitializeAsync(_titleScreenState).FireAndForget();
        }
        #endregion
    }
}

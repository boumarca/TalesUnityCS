using System.Threading.Tasks;
using Framework.Assertions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.StateStack
{
    [CreateAssetMenu(menuName = "Game Data/Game States/SceneStateData")]
    public class SceneStateData : StateData
    {
        #region Serialized Fields
        [SerializeField] public AssetReference _sceneAsset;
        #endregion

        #region Override Methods
        public override async Task<GameStateBehaviour> LoadState()
        {
            AsyncOperationHandle handle = Addressables.LoadSceneAsync(_sceneAsset);
            await handle.Task;

            AssertWrapper.IsNotNull(handle.Result, $"Failed to load Addressable asset");

            GameStateBehaviour newState = FindAnyObjectByType<GameStateBehaviour>();
            AssertWrapper.IsNotNull(newState, $"Make sure that {name} is loading an asset with a {nameof(GameStateBehaviour)} asset inside.");
            newState.InitializeState(this, handle);
            return newState;
        }
        #endregion
    }
}

using System.Threading.Tasks;
using Framework.Assertions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.StateStack
{
    [CreateAssetMenu(menuName = "Game Data/Game States/PrefabStateData")]
    public class PrefabStateData : StateData
    {
        #region Serialized Fields
        [SerializeField] private AssetReferenceGameObject _prefabAsset;
        #endregion

        #region Override Methods
        public override async Task<GameStateBehaviour> LoadState()
        {
            AsyncOperationHandle handle = Addressables.LoadAssetAsync<GameObject>(_prefabAsset);
            await handle.Task;

            AssertWrapper.IsNotNull(handle.Result, $"Failed to load Addressable asset");

            GameStateBehaviour newState = null;
            if (handle.Result is GameObject obj)
            {
                GameObject controller = Instantiate(obj);
                newState = controller.GetComponent<GameStateBehaviour>();
            }
            AssertWrapper.IsNotNull(newState, $"Make sure that {name} is loading an asset with a {nameof(GameStateBehaviour)} asset inside.");
            newState.InitializeState(this, handle);
            return newState;
        }
        #endregion
    }
}

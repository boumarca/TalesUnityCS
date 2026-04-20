using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jrpg.Menus
{
    public class BootGame : MonoBehaviour
    {
        #region Serialized Field
        [SerializeField] private AssetReference _scene;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            Addressables.LoadSceneAsync(_scene.RuntimeKey);
        }
        #endregion
    }
}

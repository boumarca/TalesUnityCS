using UnityEngine;

namespace Game.Cameras
{
    public class UiCamera : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private Camera _camera;
        #endregion

        #region MonoBehaviour Methods
        void Start()
        {
            _camera.targetTexture = ScreenCamera.Instance.UiRenderTexture;
        }
        #endregion
    }
}

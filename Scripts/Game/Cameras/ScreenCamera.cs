using System.Collections;
using System.Threading.Tasks;
using Framework.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Cameras
{
    public class ScreenCamera : GlobalSingleton<ScreenCamera>
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private FadeTransition _fadeTransition;
        [SerializeField] private RawImage _worldDisplayImage;
        [SerializeField] private RawImage _uiDisplayImage;

        [Header("Render Textures")]
        [SerializeField] private RenderTexture _worldRenderTexture;
        [SerializeField] private RenderTexture _uiRenderTexture;
        #endregion

        #region Public Properties
        public RenderTexture WorldRenderTexture => _worldRenderTexture;
        public RenderTexture UiRenderTexture => _uiRenderTexture;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            InitializeTextures();
        }
        #endregion

        #region Public Methods
        public void InstantFadeOut()
        {
            _fadeTransition.InstantFadeOut();
        }

        public async Task FadeOut()
        {
            await _fadeTransition.FadeOut();
        }

        public async Task FadeIn()
        {
            await _fadeTransition.FadeIn();
        }
        #endregion

        #region Private Methods
        private void InitializeTextures()
        {
            _worldDisplayImage.texture = WorldRenderTexture;
            _uiDisplayImage.texture = UiRenderTexture;
        }
        #endregion
    }
}

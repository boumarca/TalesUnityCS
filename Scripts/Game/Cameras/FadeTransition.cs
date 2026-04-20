using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Cameras
{
    public class FadeTransition : MonoBehaviour
    {
        #region Serialized Field
        [Header("Component References")]
        [SerializeField] private SpriteRenderer _fadePanel;

        [Header("Parameters")]
        [SerializeField] private float _duration;
        #endregion

        #region Private Fields
        private float _progress;
        #endregion

        #region Public Methods
        public void InstantFadeOut()
        {
            _fadePanel.gameObject.SetActive(true);
            _fadePanel.color = Color.black;
        }

        public async Task FadeOut()
        {
            await FadeLerp(0f, 1f);
        }

        public async Task FadeIn()
        {
            await FadeLerp(1f, 0f);
            _fadePanel.gameObject.SetActive(false);
        }
        #endregion

        #region Private Methods
        private async Task FadeLerp(float start, float end)
        {
            _progress = 0;
            _fadePanel.gameObject.SetActive(true);
            Color color = _fadePanel.color;
            color.a = start;
            _fadePanel.color = color;
            while (_progress < _duration)
            {
                await Task.Yield();
                _progress += Time.deltaTime;
                color.a = Mathf.Lerp(start, end, _progress / _duration);
                _fadePanel.color = color;
            }
        }
        #endregion
    }
}

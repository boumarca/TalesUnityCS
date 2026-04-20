using Framework.Assertions;
using UnityEngine;

namespace Framework.Animation
{
    public class SimpleAnimator : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component references")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SimpleAnimation _defaultAnimation;

        [Header("Parameters")]
        [SerializeField] private bool _playAtAwake;
        #endregion

        #region Private Fields
        private float _time;
        private int _currentIndex;
        private bool _isPlaying;
        private SimpleAnimation _currentAnimation;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            if (_playAtAwake)
                PlayAnimation(_defaultAnimation);
        }

        private void Update()
        {
            if (!_isPlaying)
                return;

            _time += Time.deltaTime;
            if (_time <= _currentAnimation.Fps)
                return;

            _time -= _currentAnimation.Fps;
            _currentIndex++;

            if (_currentIndex >= _currentAnimation.FrameCount)
            {
                if (!_currentAnimation.Loop)
                {
                    _isPlaying = false;
                    return;
                }

                _currentIndex = 0;
            }

            _spriteRenderer.sprite = _currentAnimation.GetSpriteAtIndex(_currentIndex);
        }
        #endregion

        #region Public Methods
        public void PlayAnimation(SimpleAnimation anim)
        {
            AssertWrapper.IsNotNull(anim, "Animation should not be null");

            if (anim == _currentAnimation)
                return;

            _currentAnimation = anim;
            _time = 0;
            _currentIndex = 0;
            _isPlaying = true;
            _spriteRenderer.sprite = _currentAnimation.GetSpriteAtIndex(_currentIndex);
        }
        #endregion
    }
}

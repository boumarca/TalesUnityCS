using Framework.Assertions;
using UnityEngine;

namespace Framework.Animation
{
    [CreateAssetMenu]
    public class SimpleAnimation : ScriptableObject
    {
        #region Serialized Fields
        [Header("Animation")]
        [SerializeField] private Sprite[] _sprites;

        [Header("Animation properties")]
        [SerializeField] private float _fps = 1f / 20f;
        [SerializeField] private bool _loop = true;
        #endregion

        #region Public Properties
        public float Fps => _fps;
        public bool Loop => _loop;
        public int FrameCount => _sprites.Length;
        #endregion

        #region Public Methods
        public Sprite GetSpriteAtIndex(int index)
        {
            AssertWrapper.IsIndexInRange(index, _sprites);
            return _sprites[index];
        }
        #endregion
    }
}

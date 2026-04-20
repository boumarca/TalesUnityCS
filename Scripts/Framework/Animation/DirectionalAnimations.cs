using System;
using Framework.Common;
using UnityEngine;

namespace Framework.Animation
{
    [Serializable]
    public class DirectionalAnimations
    {
        #region Public Properties
#pragma warning disable CA2235 // Mark all non-serializable fields
        [field: SerializeField] public SimpleAnimation Down { get; private set; }
        [field: SerializeField] public SimpleAnimation Left { get; private set; }
        [field: SerializeField] public SimpleAnimation Right { get; private set; }
        [field: SerializeField] public SimpleAnimation Up { get; private set; }
#pragma warning restore CA2235 // Mark all non-serializable fields
        #endregion

        #region Public Methods
        public SimpleAnimation GetAnimationForDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Down => Down,
                Direction.Left => Left,
                Direction.Right => Right,
                Direction.Up => Up,
                _ => null
            };
        }
        #endregion
    }
}

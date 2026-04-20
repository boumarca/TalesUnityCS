using System;
using UnityEngine;

namespace Framework.Common
{
#pragma warning disable CA1027 // Mark enums with FlagsAttribute
    [Serializable]
    public enum Direction
#pragma warning restore CA1027 // Mark enums with FlagsAttribute
    {
        None = 0,
        Down = 2,
        Left = 4,
        Right = 6,
        Up = 8
    }

    public static class DirectionExtensions
    {
        #region Constants
        private const double Sqrt2Over2 = 0.707106781187;
        #endregion

        #region Static Functions
        public static Direction FromVector2(Vector2 vector)
        {
            if (vector.x > Sqrt2Over2)
                return Direction.Right;
            if (vector.x < -Sqrt2Over2)
                return Direction.Left;
            if (vector.y > Sqrt2Over2)
                return Direction.Up;
            if (vector.y < -Sqrt2Over2)
                return Direction.Down;

            Debug.LogWarning("Invalid direction " + vector);
            return Direction.None;
        }
        #endregion

        #region Extension Methods
        /// <summary>
        /// Convert this Direction to a Vector2
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Vector2 corresponding to this Direction</returns>
        public static Vector2 ToVector2(this Direction direction)
        {
            return direction switch
            {
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                Direction.Up => Vector2.up,
                _ => Vector2.zero
            };
        }

        /// <summary>
        /// Convert this Direction to a Vector2Int
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Vector2Int corresponding to this Direction</returns>
        public static Vector2Int ToVector2Int(this Direction direction)
        {
            return direction switch
            {
                Direction.Down => Vector2Int.down,
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                Direction.Up => Vector2Int.up,
                _ => Vector2Int.zero
            };
        }

        /// <summary>
        /// Convert this Direction to a Vector3
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Vector3 corresponding to this Direction</returns>
        public static Vector3 ToVector3(this Direction direction)
        {
            return (Vector3)direction.ToVector2();
        }

        /// <summary>
        /// Get the opposite Direction of this Direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>The opposite Direction</returns>
        public static Direction GetOppositeDirection(this Direction direction)
        {
            return direction switch
            {
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Up => Direction.Down,
                _ => Direction.None
            };
        }
        #endregion
    }
}

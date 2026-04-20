using UnityEngine;

namespace Framework.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Rotates the vector counter-clockwise by angle.
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="angle">Angle in degrees</param>
        /// <returns></returns>
        public static Vector2 Rotate(Vector2 vector, float angle)
        {
            float radAngle = Mathf.Deg2Rad * angle;
            return new Vector2(
                vector.x * Mathf.Cos(radAngle) - vector.y * Mathf.Sin(radAngle),
                vector.x * Mathf.Sin(radAngle) + vector.y * Mathf.Cos(radAngle)
            );
        }
    }
}

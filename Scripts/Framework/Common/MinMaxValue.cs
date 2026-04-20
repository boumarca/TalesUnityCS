using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Common
{
    [Serializable]
    public struct MinMaxValue : IEquatable<MinMaxValue>
    {
        #region Serialized Fields
        [FormerlySerializedAs("MinValue")] [SerializeField]private float _minValue;
        [FormerlySerializedAs("MaxValue")] [SerializeField] private float _maxValue;
        #endregion

        #region Public Properties
        public readonly float MinValue => _minValue;
        public readonly float MaxValue => _maxValue;
        #endregion

        #region Constructors
        public MinMaxValue(float min, float max)
        {
            if(min < max)
            {
                _minValue = min;
                _maxValue = max;
            }
            else
            {
                _minValue = max;
                _maxValue = min;
            }
        }
        #endregion

        #region Override Methods
        public override readonly bool Equals(object obj) => obj is MinMaxValue asMinMax && Equals(asMinMax);

        public readonly bool Equals(MinMaxValue other) => Math.Abs(other.MinValue - MinValue) < Mathf.Epsilon && Math.Abs(other.MaxValue - MaxValue) < Mathf.Epsilon;

        public override readonly int GetHashCode() => unchecked(MinValue.GetHashCode() * 17 + MaxValue.GetHashCode());

        public static bool operator ==(MinMaxValue left, MinMaxValue right) => left.Equals(right);

        public static bool operator !=(MinMaxValue left, MinMaxValue right) => !(left == right);
        #endregion

        #region Public Methods
        /// <summary>
        /// Check to see if the value in is range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public readonly bool IsInRange(float value) => value >= MinValue && value <= MaxValue;
        #endregion
    }
}

/*
 *  Source: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
*/

using UnityEngine;
using System;

namespace Framework.Utils
{
    /// <summary>
    /// Hide the field/property ONLY if the compared property compared with the value of comparedValue returns true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class HideIfAttribute : PropertyAttribute
    {
        public string ComparedPropertyName { get; private set; }
        public object ComparedValue { get; private set; }

        /// <summary>
        /// Hide the field if the condition is met.
        /// </summary>
        /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
        /// <param name="comparedValue">The value the property is being compared to.</param>
        public HideIfAttribute(string comparedPropertyName, object comparedValue)
        {
            ComparedPropertyName = comparedPropertyName;
            ComparedValue = comparedValue;
        }
    }
}

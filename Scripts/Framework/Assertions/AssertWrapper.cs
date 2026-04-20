using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace Framework.Assertions
{
    public static class AssertWrapper
    {
        /// <summary>
        /// Return true when the condition is true. Otherwise return false.
        /// </summary>
        /// <param name="condition">true or false.</param>
        /// <param name="message">The string used to describe the result of the Assert.</param>
        [Conditional("ENABLE_ASSERTIONS")]
        public static void IsTrue(bool condition, string message)
        {
            Assert.IsTrue(condition, message);
        }

        /// <summary>
        /// Return true when the condition is false. Otherwise return false.
        /// </summary>
        /// <param name="condition">true or false.</param>
        /// <param name="message">The string used to describe the result of the Assert.</param>
        [Conditional("ENABLE_ASSERTIONS")]
        public static void IsFalse(bool condition, string message)
        {
            Assert.IsFalse(condition, message);
        }

        /// <summary>
        /// Assert that the value is not null.
        /// </summary>
        /// <param name="value">The Object or type being checked for.</param>
        /// <param name="message">The string used to describe the Assert</param>
        [Conditional("ENABLE_ASSERTIONS")]
        public static void IsNotNull<T>(T value, string message) where T : class
        {
            Assert.IsNotNull(value, message);
        }

        /// <summary>
        /// Asserts that the index is in range of the given collection.
        /// </summary>
        /// <param name="index">Index to test</param>
        /// <param name="collection">The collection to test the length of.</param>
        /// <param name="message">The string used to describe the Assert.</param>
        [Conditional("ENABLE_ASSERTIONS")]
        public static void IsIndexInRange<T>(int index, ICollection<T> collection)
        {
            IsIndexInRange(index, collection, "Index is outside the range of the array.");
        }

        /// <summary>
        /// Asserts that the index is in range of the given collection.
        /// </summary>
        /// <param name="index">Index to test</param>
        /// <param name="collection">The collection to test the length of.</param>
        /// <param name="message">The string used to describe the Assert.</param>
        [Conditional("ENABLE_ASSERTIONS")]
        public static void IsIndexInRange<T>(int index, ICollection<T> collection, string message)
        {
            Assert.IsNotNull(collection);
            Assert.IsTrue(index >= 0 && index < collection.Count, message);
        }
    }
}

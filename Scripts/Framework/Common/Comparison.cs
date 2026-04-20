using UnityEngine;

namespace Framework.Common
{
    public enum Comparison
    {
        EqualTo,
        NotEqualTo,
        LessThan,
        LessThanOrEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
    }

    public static class ComparisonExtensions
    {
        #region Static Functions
        public static bool CompareValues(int a, int b, Comparison operation)
        {
            switch (operation)
            {
                case Comparison.EqualTo:
                    return a == b;
                case Comparison.NotEqualTo:
                    return a != b;
                case Comparison.LessThan:
                    return a < b;
                case Comparison.LessThanOrEqualTo:
                    return a <= b;
                case Comparison.GreaterThan:
                    return a > b;
                case Comparison.GreaterThanOrEqualTo:
                    return a >= b;
            }

            Debug.LogError($"Operation is invalid value. This should not happen");
            return false;
        }

        public static bool CompareValues(object a, object b, Comparison operation)
        {
            switch (operation)
            {
                case Comparison.EqualTo:
                    return Equals(a, b);
                case Comparison.NotEqualTo:
                    return !Equals(a, b);

                //These cases does not apply for objects.
                case Comparison.LessThan:
                case Comparison.LessThanOrEqualTo:
                case Comparison.GreaterThan:
                case Comparison.GreaterThanOrEqualTo:
                default:
                    return false; 
            }
        }
        #endregion
    }
}

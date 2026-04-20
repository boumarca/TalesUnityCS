namespace Framework.Extensions
{
    public static class Int32Extensions
    {
        public static bool IsBetween(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
    }
}

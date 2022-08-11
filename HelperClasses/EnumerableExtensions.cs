namespace HelperClasses
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T>? input, Action<T>? action)
        {
            if (input == null || action == null)
            {
                return;
            }

            foreach (var obj in input)
            {
                action.Invoke(obj);
            }
        }

        public static bool None<T>(this IEnumerable<T> input, Func<T, bool> func)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return false;
        }
    }
}

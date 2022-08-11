namespace HelperClasses
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
        {
            foreach (var obj in input)
            {
                action.Invoke(obj);
            }
        }

        public static bool None<T>(this IEnumerable<T> input, Func<T, bool> func) => !input.Any(func);
    }
}

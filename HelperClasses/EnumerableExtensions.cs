using System;
using System.Collections.Generic;

namespace HelperClasses
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
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
    }
}

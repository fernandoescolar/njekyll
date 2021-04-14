using System.Collections.Generic;
using System.Linq;

namespace NJekyll.Utilities
{
	public static class EnumerableExtensions
	{
        public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> enumerable, int size)
        {
            var list = new List<T>(size);
            var counter = 0;
            foreach (var item in enumerable)
            {
                list.Add(item);
                if (++counter == size)
                {
                    yield return list;
                    list = new List<T>(size);
                    counter = 0;
                }
            }

            if (list.Any()) yield return list.AsEnumerable();
        }
    }
}

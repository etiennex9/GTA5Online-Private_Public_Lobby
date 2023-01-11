using System.Collections.Generic;
using System.Linq;

namespace CodeSwine_Solo_Public_Lobby.Extensions
{
    public static class EnumerableExtensions
    {
        public static IOrderedEnumerable<T> Sort<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => x);
        }
    }
}

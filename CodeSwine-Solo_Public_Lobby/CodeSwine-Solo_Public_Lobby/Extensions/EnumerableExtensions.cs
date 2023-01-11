using System.Collections.Generic;
using System.Linq;

namespace GTA5_Private_Public_Lobby.Extensions
{
    public static class EnumerableExtensions
    {
        public static IOrderedEnumerable<T> Sort<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => x);
        }
    }
}

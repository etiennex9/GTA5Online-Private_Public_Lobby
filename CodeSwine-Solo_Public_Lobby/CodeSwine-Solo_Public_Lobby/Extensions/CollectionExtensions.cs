using System.Collections.Generic;

namespace CodeSwine_Solo_Public_Lobby.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                destination.Add(item);
            }
        }

        public static void RemoveRange<T>(this ICollection<T> destination, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                destination.Remove(item);
            }
        }
    }
}

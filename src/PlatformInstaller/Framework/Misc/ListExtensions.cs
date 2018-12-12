using System.Collections;
using System.Linq;

static class ListExtensions
{
    public static bool Contains<T>(this IEnumerable list) => list.OfType<T>().Any();
}
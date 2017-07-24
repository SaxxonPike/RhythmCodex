using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhythmCodex.Extensions
{
    public static class EnumerableExtensions
    {
        public static IList<T> AsList<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable as IList<T>) ?? enumerable.ToList();
        }
    }
}

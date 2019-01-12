using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    public static class Reducer
    {
        public static T[] ReduceRhythm<T>(this IEnumerable<T> source, Predicate<T> canReduce)
        {
            var result = source.ToArray().AsSpan();
            var count = result.Length;
            var fail = false;

            while (!fail && count > 1)
            {
                for (var p = 2; p <= count; p++)
                {
                    fail = false;

                    if (count % p == 0)
                    {
                        for (var j = 0; j < count; j++)
                        {
                            if (j % p == 0)
                                continue;

                            if (canReduce(result[j]))
                                continue;

                            fail = true;
                            break;
                        }
                    }
                    else
                    {
                        fail = true;
                    }

                    if (fail)
                        continue;

                    var newCount = count / p;
                    var index = 0;

                    for (var j = count - p; j >= 0; j -= p)
                    {
                        result[index] = result[j];
                        index++;
                    }

                    count = newCount;
                    result = result.Slice(0, newCount);
                    break;
                }
            }

            return result.ToArray();
        }
    }
}
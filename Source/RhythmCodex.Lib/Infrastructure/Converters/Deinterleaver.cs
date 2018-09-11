using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.Infrastructure.Converters
{
    public class Deinterleaver : IDeinterleaver
    {
        public IList<IList<T>> Deinterleave<T>(IEnumerable<T> data, int interleave, int streamCount)
        {
            var result = Enumerable.Range(0, streamCount).Select(i => new List<T>()).Cast<IList<T>>().ToList();
            var index = 0;
            var channel = 0;
            
            foreach (var i in data)
            {
                result[channel].Add(i);
                
                index++;
                
                if (index >= interleave)
                {
                    index = 0;
                    channel++;
                }

                if (channel >= streamCount)
                    channel = 0;
            }

            return result;
        }
    }
}
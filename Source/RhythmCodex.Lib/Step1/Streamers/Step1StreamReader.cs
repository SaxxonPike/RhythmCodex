using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Step1.Models;

namespace RhythmCodex.Step1.Streamers
{
    [Service]
    public class Step1StreamReader : IStep1StreamReader
    {
        public IList<Step1Chunk> Read(Stream stream)
        {
            return ReadInternal(stream).ToList();
        }

        private IEnumerable<Step1Chunk> ReadInternal(Stream stream)
        {
            var reader = new BinaryReader(stream);
            
            while (true)
            {
                var length = reader.ReadInt32();

                if (length < 4)
                    yield break;

                yield return new Step1Chunk
                {
                    Data = reader.ReadBytes(length - 4)
                };                            
            }
        }
    }
}
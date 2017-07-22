using System;
using System.IO;
using NUnit.Framework;

namespace RhythmCodex.Ssq.Streamers
{
    [TestFixture]
    public abstract class ChunkStreamBaseTests<TSubject> : BaseTestFixture<TSubject> where TSubject : class
    {
        protected static byte[] PrepareChunk(short param0, short param1, byte[] data)
        {
            var newLength = ((data.Length + 3) >> 2) << 2;
            var newData = new byte[newLength];
            Array.Copy(data, newData, data.Length);
            
            return PrepareChunk(newLength + 8, param0, param1, newData);
        }

        protected static byte[] PrepareChunk(int totalLength, short param0, short param1, byte[] data)
        {
            using (var mem = new MemoryStream())
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(totalLength);
                writer.Write(param0);
                writer.Write(param1);
                writer.Write(data);
                writer.Flush();
                return mem.ToArray();
            }
        }
    }
}

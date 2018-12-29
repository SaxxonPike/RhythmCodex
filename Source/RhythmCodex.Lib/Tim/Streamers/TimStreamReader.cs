using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Tim.Models;

namespace RhythmCodex.Tim.Streamers
{
    public class TimStreamReader : ITimStreamReader
    {
        public TimImage Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            if (reader.ReadInt32() != 0x00000010)
                throw new RhythmCodexException("Unrecognized TIM image identifier.");

            var cluts = new List<TimPalette>();
            var result = new TimImage
            {
                ImageType = reader.ReadInt32(),
                Cluts = cluts
            };

            switch (result.ImageType)
            {
                case 0x00000008:
                case 0x00000009:
                    cluts.AddRange(ReadCluts(stream));
                    break;
            }

            var length = reader.ReadInt32();
            result.OriginX = reader.ReadInt16();
            result.OriginY = reader.ReadInt16();
            result.Stride = reader.ReadInt16();
            result.Height = reader.ReadInt16();
            var padding = length - result.Stride * result.Height;
            result.Data = reader.ReadBytes(length);

            if (padding > 0)
                reader.ReadBytes(padding);

            return result;
        }
        
        private IEnumerable<TimPalette> ReadCluts(Stream stream)
        {
            var reader = new BinaryReader(stream);

            var length = reader.ReadInt32();
            var originX = reader.ReadInt16();
            var originY = reader.ReadInt16();
            int numColors = reader.ReadInt16();
            int numCluts = reader.ReadInt16();
            var padding = length - numCluts * numColors * 2;

            for (var i = 0; i < numCluts; i++)
            {
                var entries = new short[numColors];
                var clut = new TimPalette
                {
                    OriginX = originX,
                    OriginY = originY,
                    Entries = entries
                };
                
                for (var j = 0; j < numColors; j++)
                    entries[j] = reader.ReadInt16();

                yield return clut;
            }

            if (padding > 0)
                reader.ReadBytes(padding);
        }
    }
}
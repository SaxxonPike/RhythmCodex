using System;
using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Flac.Converters;

public interface IFlacDecoder
{
    Sound? Decode(Stream stream);
    Memory<byte> DecodeFrame(Stream stream, int blockSize);
}
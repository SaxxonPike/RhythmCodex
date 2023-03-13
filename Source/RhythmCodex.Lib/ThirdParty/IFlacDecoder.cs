using System;
using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ThirdParty;

public interface IFlacDecoder
{
    ISound Decode(Stream stream);
    Memory<byte> DecodeFrame(Stream stream, int blockSize);
}
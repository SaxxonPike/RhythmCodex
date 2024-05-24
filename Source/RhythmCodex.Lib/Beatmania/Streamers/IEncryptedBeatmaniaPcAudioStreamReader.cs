using System;
using System.IO;

namespace RhythmCodex.Beatmania.Streamers;

public interface IEncryptedBeatmaniaPcAudioStreamReader
{
    Memory<byte> Decrypt(Stream source, long length);
}
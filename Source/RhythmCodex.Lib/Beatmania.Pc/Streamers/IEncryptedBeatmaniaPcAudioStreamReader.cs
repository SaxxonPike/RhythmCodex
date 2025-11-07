using System;
using System.IO;

namespace RhythmCodex.Beatmania.Pc.Streamers;

public interface IEncryptedBeatmaniaPcAudioStreamReader
{
    Memory<byte> Decrypt(Stream source, long length);
}
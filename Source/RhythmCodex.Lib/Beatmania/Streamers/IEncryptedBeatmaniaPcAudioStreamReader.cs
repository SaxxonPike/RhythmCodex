using System.IO;

namespace RhythmCodex.Beatmania.Streamers;

public interface IEncryptedBeatmaniaPcAudioStreamReader
{
    byte[] Decrypt(Stream source, long length);
}
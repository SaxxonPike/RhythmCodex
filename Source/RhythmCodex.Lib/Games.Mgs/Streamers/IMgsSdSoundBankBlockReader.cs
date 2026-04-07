using System.IO;
using RhythmCodex.Games.Mgs.Models;

namespace RhythmCodex.Games.Mgs.Streamers;

public interface IMgsSdSoundBankBlockReader
{
    MgsSdSoundBankBlock Read(Stream stream);
}
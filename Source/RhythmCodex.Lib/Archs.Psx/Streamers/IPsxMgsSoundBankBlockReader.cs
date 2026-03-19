using System.IO;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Streamers;

public interface IPsxMgsSoundBankBlockReader
{
    PsxMgsSoundBankBlock Read(Stream stream);
}
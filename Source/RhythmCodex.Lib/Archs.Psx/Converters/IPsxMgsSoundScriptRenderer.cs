using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxMgsSoundScriptRenderer
{
    Sound Render(PsxMgsSoundScript script, List<PsxMgsSoundBankEntryWithData> soundBank, int sampleRate);
}
using RhythmCodex.Games.Beatmania.Pc.Models;
using RhythmCodex.Games.Beatmania.Pc.Providers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Wav.Converters;

namespace RhythmCodex.Games.Beatmania.Pc.Converters;

[Service]
public class BeatmaniaPcAudioDecoder(IWavDecoder wavDecoder) : IBeatmaniaPcAudioDecoder
{
    public Sound? Decode(BeatmaniaPcAudioEntry entry)
    {
        using var wavDataMem = new ReadOnlyMemoryStream(entry.Data);
        
        var result = wavDecoder.Decode(wavDataMem);
        if (result == null)
            return null;

        var panning = entry.Panning;
        if (panning is > 0x7F or < 0x01)
            panning = 0x40;

        var volume = entry.Volume switch
        {
            < 0x01 => 0x01,
            > 0xFF => 0xFF,
            _ => entry.Volume
        };

        result[NumericData.Panning] = (panning - 1.0d) / 126.0d;
        result[NumericData.Volume] = BeatmaniaPcConstants.VolumeTable.Span[volume];
        result[NumericData.Channel] = entry.Channel;
        result[NumericData.SourceVolume] = entry.Volume;
        result[NumericData.SourcePanning] = entry.Panning;

        return result;
    }
}
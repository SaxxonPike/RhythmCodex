using RhythmCodex.Beatmania.Models;
using RhythmCodex.Beatmania.Providers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Beatmania.Converters;

[Service]
public class BeatmaniaPcAudioDecoder(IWavDecoder wavDecoder) : IBeatmaniaPcAudioDecoder
{
    public Sound? Decode(BeatmaniaPcAudioEntry entry)
    {
        using var wavDataMem = new ReadOnlyMemoryStream(entry.Data);
        var result = wavDecoder.Decode(wavDataMem);

        var panning = entry.Panning;
        if (panning is > 0x7F or < 0x01)
            panning = 0x40;

        var volume = entry.Volume;
        if (volume < 0x01)
            volume = 0x01;
        else if (volume > 0xFF)
            volume = 0xFF;

        result[NumericData.Panning] = (panning - 1.0d) / 126.0d;
        result[NumericData.Volume] = BeatmaniaPcConstants.VolumeTable.Span[volume];
        result[NumericData.Channel] = entry.Channel;
        result[NumericData.SourceVolume] = entry.Volume;
        result[NumericData.SourcePanning] = entry.Panning;

        return result;
    }
}
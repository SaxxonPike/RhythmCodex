using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Mixer.Converters;
using RhythmCodex.Sounds.Mixer.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

/// <summary>
/// Acts as a mixer for sounds extracted from a Djmain system.
/// </summary>
[Service]
public class DjmainMixer : StereoMixer, IDjmainMixer
{
    /// <summary>
    /// Returns unity gain. All mixing is done per sound in <see cref="GetSampleMix"/>.
    /// </summary>
    protected override MixAmount GetMasterMix(MixState state) =>
        new(FromLeft: (ToLeft: 1, ToRight: 0), FromRight: (ToLeft: 0, ToRight: 1));

    /// <inheritdoc />
    protected override MixAmount GetSampleMix(Sample? left, Sample? right, MixState state)
    {
        //
        // Volume table is 128 values, with 0x00 being the loudest. 0x7F is silent.
        //

        var volTabIdx = Math.Clamp((int)(state.Sound?[NumericData.SourceVolume] ?? BigRational.Zero), 0x00, 0x7F);

        //
        // Panning table is 15 values, 0x1 to 0xF. We use the value directly from imported sample
        // data, but BGM events can override. The BGM panning value is stereo-flipped. In the event
        // that panning data is missing altogether, default to center (0x8).
        //
        
        int panTableRaw;

        if (state.EventData?[NumericData.SourcePanning] is { } bgmPanning)
            panTableRaw = 0x10 - ((int)bgmPanning & 0xF);
        else if (state.Sound?[NumericData.SourcePanning] is { } soundPanning)
            panTableRaw = (int)soundPanning & 0xF;
        else
            panTableRaw = 0x8;

        var panTabIdx = Math.Clamp(panTableRaw, 0x1, 0xF) - 1;

        //
        // Normalize the table values to range 0.0-1.0.
        //
        
        var finalVol = DjmainConstants.VolumeRom.Span[volTabIdx] / (double)0x7FFF;
        var finalPanLeft = DjmainConstants.PanRom.Span[panTabIdx] / (double)0x7FFF;
        var finalPanRight = DjmainConstants.PanRom.Span[0xE - panTabIdx] / (double)0x7FFF;

        return new MixAmount(
            FromLeft: (
                ToLeft: finalVol * finalPanLeft,
                ToRight: 0
            ),
            FromRight: (
                ToLeft: 0,
                ToRight: finalVol * finalPanRight
            )
        );
    }
}

public interface IDjmainMixer : IStereoMixer
{
}
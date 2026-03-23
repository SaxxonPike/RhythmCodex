using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public class PsxMgsSoundScriptRenderer(
    IVagSplitter vagSplitter,
    IAudioDsp audioDsp)
    : IPsxMgsSoundScriptRenderer
{
    /// <summary>
    /// Frequency table used by the Metal Gear Solid sound system.
    /// </summary>
    private readonly Lazy<ReadOnlyMemory<int>> _freqTbl = new(() => (int[])
    [
        0x010B, 0x011B, 0x012C, 0x013E, 0x0151, 0x0165, 0x017A, 0x0191, 0x01A9, 0x01C2, 0x01DD, 0x01F9,
        0x0217, 0x0237, 0x0259, 0x027D, 0x02A3, 0x02CB, 0x02F5, 0x0322, 0x0352, 0x0385, 0x03BA, 0x03F3,
        0x042F, 0x046F, 0x04B2, 0x04FA, 0x0546, 0x0596, 0x05EB, 0x0645, 0x06A5, 0x070A, 0x0775, 0x07E6,
        0x085F, 0x08DE, 0x0965, 0x09F4, 0x0A8C, 0x0B2C, 0x0BD6, 0x0C8B, 0x0D4A, 0x0E14, 0x0EEA, 0x0FCD,
        0x10BE, 0x11BD, 0x12CB, 0x13E9, 0x1518, 0x1659, 0x17AD, 0x1916, 0x1A94, 0x1C28, 0x1DD5, 0x1F9B,
        0x217C, 0x237A, 0x2596, 0x27D2, 0x2A30, 0x2CB2, 0x2F5A, 0x322C, 0x3528, 0x3850, 0x3BAC, 0x3F36,

        0x0021, 0x0023, 0x0026, 0x0028, 0x002A, 0x002D, 0x002F, 0x0032, 0x0035, 0x0038, 0x003C, 0x003F,
        0x0042, 0x0046, 0x004B, 0x004F, 0x0054, 0x0059, 0x005E, 0x0064, 0x006A, 0x0070, 0x0077, 0x007E,
        0x0085, 0x008D, 0x0096, 0x009F, 0x00A8, 0x00B2, 0x00BD, 0x00C8, 0x00D4, 0x00E1, 0x00EE, 0x00FC
    ]);

    /// <inheritdoc />
    public Sound Render(
        PsxMgsSoundScript script,
        List<PsxMgsSoundBankEntryWithData> soundBank,
        int sampleRate)
    {
        var sounds = script.Channels
            .Select(kv => RenderPackets(kv.Value, soundBank, sampleRate))
            .ToList();

        var output = sounds.Count switch
        {
            0 => new Sound(),
            1 => sounds[0],
            _ => audioDsp.Mix(sounds)
        };

        return output;
    }

    /// <summary>
    /// Renders an MGS audio packet stream.
    /// </summary>
    /// <param name="packets">
    /// Packets to render.
    /// </param>
    /// <param name="soundBank">
    /// Sound bank that will be used as the audio data source.
    /// </param>
    /// <param name="sampleRate">
    /// Sampling rate for the output mix.
    /// </param>
    /// <returns></returns>
    private Sound RenderPackets(
        List<PsxMgsSoundTablePacket> packets,
        List<PsxMgsSoundBankEntryWithData> soundBank,
        int sampleRate)
    {
        Sample? sourceSample = null;
        var sourceSampleOffset = 0f;
        var sourceSampleRate = 0f;
        var sourceFineTune = 0;
        var sourceTranspose = 0;
        var sampleId = -1;
        var resolution = 1f;

        var result = new SoundBuilder(2);

        var currentVolume = 1.0f;
        var targetVolume = 1.0f;
        var volumeFadeTime = 0f;

        var currentPanning = 0.5f;
        var targetPanning = 0.5f;
        var panningFadeTime = 0f;

        var currentNote = 0f;
        var targetNote = 0f;
        var notePortTime = 0f;
        var playedNote = 0;
        var pendingSilence = 0;

        var setSample = false;
        var ad = 0;
        var sr = 0;
        var transpose = 0;
        var detune = 0;
        var procMs = 0f;
        var noteOn = false;
        var lastNoteOn = false;
        var finished = false;
        var sampleL = result.Samples[0];
        var sampleR = result.Samples[1];

        foreach (var packet in packets)
        {
            switch (packet.Command)
            {
                case PsxMgsSoundTablePacketType.SetTempo:
                {
                    resolution = packet.Data2 / 128f;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetId:
                {
                    sampleId = packet.Data2;
                    setSample = true;
                    continue;
                }
                case PsxMgsSoundTablePacketType.MoveVolume:
                {
                    targetVolume = packet.Data3 / 255f;
                    volumeFadeTime = packet.Data2 * 10 * resolution;
                    continue;
                }
                case PsxMgsSoundTablePacketType.ChangeVolume:
                {
                    targetVolume = currentVolume = packet.Data2 / 255f;
                    volumeFadeTime = 0;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetSustainRelease:
                {
                    sr = ~packet.Data2 & 0x7F;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetPanning:
                {
                    targetPanning = currentPanning = (packet.Data3 & 0xF) / 15f;
                    panningFadeTime = 0;
                    continue;
                }
                case PsxMgsSoundTablePacketType.MovePanning:
                {
                    targetPanning = (packet.Data3 & 0xF) / 15f;
                    panningFadeTime = packet.Data2 * 10 * resolution;
                    continue;
                }
                case PsxMgsSoundTablePacketType.Transpose:
                {
                    transpose = unchecked((sbyte)packet.Data2);
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetDetune:
                {
                    detune = unchecked((sbyte)packet.Data2);
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetSwp:
                {
                    // todo
                    notePortTime = packet.Data2 * 10 * resolution;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetRest:
                {
                    noteOn = false;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetTie:
                {
                    procMs += packet.Data2 * 10 * resolution;
                    continue;
                }
                case PsxMgsSoundTablePacketType.EndBlock:
                {
                    finished = true;
                    procMs = 0;
                    break;
                }
                case var data1 when (int)data1 >= 0x80:
                {
                    // unsupported command
                    continue;
                }
                default:
                {
                    noteOn = true;
                    procMs += packet.Data2 * 10 * resolution;
                    playedNote = packet.Data1;
                    break;
                }
            }

            //
            // If the sample has changed, convert it.
            //

            if (setSample)
            {
                setSample = false;
                var sample = soundBank.FirstOrDefault(x => x.Index == sampleId);
                sourceFineTune = unchecked((sbyte)(sample?.Entry.Tune ?? 0));
                sourceTranspose = unchecked((sbyte)(sample?.Entry.Note ?? 0));
                sourceSampleOffset = 0;

                sourceSample = sample == null
                    ? null
                    : vagSplitter
                        .Split(new VagChunk { Channels = 1, Data = sample.Data })
                        .FirstOrDefault();
            }

            //
            // Starting frequency is calculated at note-on.
            //

            if (!lastNoteOn && noteOn)
            {
                sourceSampleRate = CalculateFrequency(
                    playedNote,
                    transpose + sourceTranspose,
                    detune + sourceFineTune
                );
            }

            //
            // Determine how many wave samples will be generated.
            //

            var sampleCount = (int)MathF.Truncate(sampleRate * procMs / 1000);
            using var buffer = MemoryPool<float>.Shared.Rent(sampleCount);
            var sampleFloats0 = buffer.Memory.Span[..sampleCount];
            var sampleFloats1 = buffer.Memory.Span[..sampleCount];
            var sourceSampleSpan = sourceSample != null ? sourceSample.Data.Span : [];

            //
            // Populate the output.
            //

            if (sourceSampleOffset < sourceSampleSpan.Length)
            {
                if (sourceSample != null && pendingSilence > 0)
                {
                    using var silenceBuffer = MemoryPool<float>.Shared.Rent(pendingSilence);
                    silenceBuffer.Memory.Span[..pendingSilence].Clear();
                    var silenceSpan = silenceBuffer.Memory.Span[..pendingSilence];
                    sampleL.Append(silenceSpan);
                    sampleR.Append(silenceSpan);
                    pendingSilence = 0;
                }

                var populatedSampleCount = sampleCount;

                for (var i = 0; i < sampleCount; i++)
                {
                    //
                    // Process pending automation events.
                    //

                    ProcessFadeVars(ref currentVolume, targetVolume, ref volumeFadeTime, sampleRate);
                    ProcessFadeVars(ref currentPanning, targetPanning, ref panningFadeTime, sampleRate);

                    //
                    // If we are at the end of the sample, fill the buffer with silence.
                    //

                    var sourceSampleOffset0 = (int)MathF.Truncate(sourceSampleOffset);
                    var sourceSampleOffset1 = sourceSampleOffset0 + 1;

                    if (sourceSampleOffset1 > sourceSampleSpan.Length)
                    {
                        pendingSilence += sampleCount - i;
                        populatedSampleCount = i;
                        break;
                    }

                    //
                    // Calculate position for linear interpolation.
                    // TODO: Playstation SPU actually uses Gaussian interpolation.
                    //

                    var sourceSample0 = sourceSampleOffset0 < sourceSampleSpan.Length
                        ? sourceSampleSpan[sourceSampleOffset0]
                        : 0;

                    var sourceSample1 = sourceSampleOffset1 < sourceSampleSpan.Length
                        ? sourceSampleSpan[sourceSampleOffset1]
                        : 0;

                    var weight = sourceSampleOffset - sourceSampleOffset0;

                    //
                    // Calculate interpolated sample value (Lerp function.)
                    //
                    
                    var sourceInterpolatedSample = sourceSample0 + weight * (sourceSample1 - sourceSample0);

                    //
                    // Calculate gain for left/right channels from panning and volume.
                    //

                    var sampleDataVec = new Vector2(sourceInterpolatedSample) *
                                        Vector2.SquareRoot(new Vector2(1 - currentPanning, currentPanning)) *
                                        currentVolume;

                    //
                    // Write the sample.
                    //

                    sampleFloats0[i] = sampleDataVec.X;
                    sampleFloats1[i] = sampleDataVec.Y;
                    sourceSampleOffset += sourceSampleRate / sampleRate;
                }

                //
                // Append the rendered samples to the result.
                //

                sampleL.Append(sampleFloats0[..populatedSampleCount]);
                sampleR.Append(sampleFloats1[..populatedSampleCount]);
                lastNoteOn = noteOn;
            }

            if (finished)
                break;

            procMs -= sampleCount * 1000f / sampleRate;
        }

        result[NumericData.Id] = sampleId;
        return result.ToSound();
    }

    /// <summary>
    /// Process automation.
    /// </summary>
    /// <param name="value">
    /// Current value.
    /// </param>
    /// <param name="fadeTarget">
    /// Target value.
    /// </param>
    /// <param name="fadeMs">
    /// Amount of time remaining in the automation.
    /// </param>
    /// <param name="sampleRate">
    /// Output sampling rate.
    /// </param>
    private static void ProcessFadeVars(ref float value, float fadeTarget, ref float fadeMs, int sampleRate)
    {
        if (fadeMs <= 0)
        {
            value = fadeTarget;
            fadeMs = 0;
            return;
        }

        if (MathF.Abs(value - fadeTarget) <= float.Epsilon)
        {
            fadeMs = 0;
            value = fadeTarget;
            return;
        }

        var diffPerMs = (fadeTarget - value) / fadeMs;
        var sampleTime = 1000f / sampleRate;
        var adjust = diffPerMs * sampleTime;
        fadeMs -= sampleTime;

        value += adjust;
    }

    /// <summary>
    /// Calculates the playback frequency of a note.
    /// </summary>
    /// <param name="note">
    /// Base semitone.
    /// </param>
    /// <param name="macro">
    /// Sample tuning (semitones.)
    /// </param>
    /// <param name="micro">
    /// Sample tuning (1/128ths of semitones, signed.)
    /// </param>
    private float CalculateFrequency(int note, int macro, int micro)
    {
        //
        // Determine the coarse and fine-tune components.
        //

        var noteTune = ((note + macro) << 8) + (micro << 1);
        var adjustedTune = noteTune;
        var fineTune = unchecked((byte)adjustedTune);
        var coarseTune = (adjustedTune >> 8) & 0x7F;

        //
        // Use linear interpolation to convert the fine-tune value to whole cycles.
        //

        var coarseFreqTable = _freqTbl.Value.Span;
        float coarseTuneCycles = coarseFreqTable[coarseTune];
        var toneScale = coarseFreqTable[coarseTune + 1] - coarseTuneCycles;

        if (toneScale < 0)
            toneScale = 0xC9;

        var fineTuneCycles = fineTune / 128f * toneScale;
        var totalCycles = Math.Clamp(MathF.Round(coarseTuneCycles + fineTuneCycles), 0, 16384f);

        var result = 44100f * totalCycles / 0x1000;
        return result;
    }
}
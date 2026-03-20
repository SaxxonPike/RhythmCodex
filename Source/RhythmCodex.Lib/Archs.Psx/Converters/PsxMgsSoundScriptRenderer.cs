using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public class PsxMgsSoundScriptRenderer(
    IVagSplitter vagSplitter,
    IBeatmaniaPs2FrequencyConverter beatmaniaPs2FrequencyConverter)
    : IPsxMgsSoundScriptRenderer
{
    private Lazy<ReadOnlyMemory<int>> freqTbl = new(() => (int[])
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

    public Sound Render(
        PsxMgsSoundScript script,
        List<PsxMgsSoundBankEntryWithData> soundBank,
        int track,
        int sampleRate)
    {
        var samples = script.Channels
            .Select(kv => RenderInternal(kv.Value, soundBank, track, sampleRate))
            .ToList();

        var result = new Sound
        {
            Samples = samples
        };

        return result;
    }

    private Sample RenderInternal(
        List<PsxMgsSoundTablePacket> packets,
        List<PsxMgsSoundBankEntryWithData> soundBank,
        int track,
        int sampleRate)
    {
        Sample? sourceSample = null;
        var sourceSampleOffset = 0f;
        var sourceSampleRate = 0f;
        var sourceFineTune = 0;
        var sourceTranspose = 0;
        var sampleId = -1;
        var resolution = 1f;

        var result = new SampleBuilder();

        var currentVolume = 0f;
        var targetVolume = 0f;
        var volumeFadeTime = 0f;

        var currentPanning = 0f;
        var targetPanning = 0f;
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

        PsxMgsSoundBankEntryWithData? sample = null;

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
                    procMs = Math.Max(panningFadeTime, volumeFadeTime);
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

            if (procMs <= 0)
                continue;

            //
            // If the sample has changed, convert it.
            //

            if (setSample)
            {
                setSample = false;
                sample = soundBank.FirstOrDefault(x => x.Index == sampleId);
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

            var sampleCount = (int)MathF.Truncate(sampleRate * 1000 / procMs);
            using var buffer = MemoryPool<float>.Shared.Rent(sampleCount);
            var sampleFloats = buffer.Memory.Span[..sampleCount];
            var sourceSampleSpan = sourceSample != null ? sourceSample.Data.Span : [];

            //
            // Populate the output.
            //

            if (sourceSample != null && pendingSilence > 0)
            {
                using var silenceBuffer = MemoryPool<float>.Shared.Rent(pendingSilence);
                silenceBuffer.Memory.Span[..pendingSilence].Clear();
                result.Append(silenceBuffer.Memory.Span[..pendingSilence]);
                pendingSilence = 0;
            }

            var populatedSampleCount = 0;

            for (var i = 0; i < sampleCount; i++)
            {
                var sourceSampleOffsetInt = (int)MathF.Truncate(sourceSampleOffset);
                if (sourceSampleOffsetInt >= sourceSampleSpan.Length)
                {
                    pendingSilence += sampleCount - i;
                    populatedSampleCount = i;
                    break;
                }

                sampleFloats[i] = sourceSampleSpan[sourceSampleOffsetInt];
                sourceSampleOffset += sourceSampleRate / sampleRate;
            }

            result.Append(sampleFloats[..populatedSampleCount]);
            lastNoteOn = noteOn;
            procMs = 0;
        }

        result[NumericData.Id] = sampleId;
        return result.ToSample();
    }

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

        var coarseFreqTable = freqTbl.Value.Span;
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
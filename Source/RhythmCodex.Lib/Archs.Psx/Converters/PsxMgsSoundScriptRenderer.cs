using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Helpers;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public sealed class PsxMgsSoundScriptRenderer(
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

    /// <summary>
    /// Panning table used by the Metal Gear Solid sound system.
    /// </summary>
    private readonly Lazy<ReadOnlyMemory<byte>> _panTbl = new(() => (byte[])
    [
        0, 2, 4, 7, 10, 13, 16, 20, 24, 28, 32, 36, 40, 45,
        50, 55, 60, 65, 70, 75, 80, 84, 88, 92, 96, 100, 104, 107,
        110, 112, 114, 116, 118, 120, 122, 123, 124, 125, 126, 127, 127
    ]);

    [Flags]
    private enum PendingAction
    {
        SetSampleIndex = 1 << 0,
        RebuildSpuEnvelope = 1 << 1
    }

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
        var resolutionMs = 10f;

        var sampleLoopStart = -1;
        var sampleLoopEnd = -1;

        var result = new SoundBuilder(2);

        var currentVolume = 1.0f;
        var currentPanning = 0.5f;
        var currentPitch = 0f;
        var volumeScale = 1.0f;

        var playedNote = 0;
        var pendingSilence = 0;
        PendingAction action = default;

        var spuVolumeEnvelope = CalculateSpuEnvelope(0, 0, new Vector2(0, 1), 0);
        var panTbl = _panTbl.Value.Span;

        Envelope? volumeEnvelope = null;
        Envelope? panningEnvelope = null;
        Envelope? pitchEnvelope = null;

        var spuAMode = 0;
        var spuAr = 0;
        var spuDr = 0;
        var spuSMode = 0;
        var spuSr = 0;
        var spuSl = 0;
        var spuRMode = 0;
        var spuRr = 0;

        var portamentoTime = 0f;

        var coarseTune = 0;
        var fineTune = 0;
        var delayMs = 0f;
        var finished = false;
        var sampleL = result.Samples[0];
        var sampleR = result.Samples[1];
        var disableDefaultPan = false;

        var effectsEnabled = false;

        foreach (var packet in packets)
        {
            //
            // Process one packet. The packet command will either be a special script command
            // (for values >= 0x80) or a note command (for values < 0x80). Some commands implicitly
            // add a delay until the next packet is processed, but notes always will.
            //

            switch (packet.Command)
            {
                case var data1 when (int)data1 < 0x80:
                {
                    //
                    // Note-on command.
                    //

                    AttackSpuEnvelope(spuVolumeEnvelope);
                    delayMs += packet.Data2 * resolutionMs;
                    playedNote = packet.Data1;
                    currentVolume = (packet.Data4 & 0x7F) / 127f;
                    sourceSampleRate = CalculateFrequency(
                        playedNote,
                        coarseTune + sourceTranspose,
                        fineTune + sourceFineTune
                    );
                    sourceSampleOffset = 0;

                    break;
                }
                case PsxMgsSoundTablePacketType.SetTimeResolution:
                {
                    //
                    // Sets the time resolution (how much time each tick represents.)
                    //

                    resolutionMs = MathF.Max(1f, packet.Data2) * 10.4f / 255f;
                    continue;
                }
                case PsxMgsSoundTablePacketType.AutomateTimeResolution:
                {
                    //
                    // TODO: I don't know how to go about implementing this command
                    // so the changes will be instantaneous for now.
                    //

                    resolutionMs = MathF.Max(1f, packet.Data2) * 10.4f / 255f;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetSoundBankIndex:
                {
                    //
                    // Sets which sound bank entry to use for sample data.
                    //

                    sampleId = packet.Data2;
                    action |= PendingAction.SetSampleIndex;

                    // In order for overrides to not get overridden themselves
                    // by defaults, we have to apply the sample change in this iteration.

                    break;
                }
                case PsxMgsSoundTablePacketType.AutomateVolume:
                {
                    //
                    // Starts a software volume envelope.
                    //

                    volumeEnvelope = new Envelope(
                        new Vector2(0, currentVolume),
                        new Vector2(packet.Data2 * resolutionMs, packet.Data3 / 255f)
                    );
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetVolumeScale:
                {
                    //
                    // Sets the software volume scale. This will impact
                    // the calculated volume but does not change anything
                    // with the simulated SPU.
                    //

                    volumeEnvelope = null;
                    volumeScale = packet.Data2 / 255f;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetAttackDecay:
                {
                    //
                    // Sets the attack/decay + sustain level SPU registers.
                    //

                    spuAMode = 1;
                    spuAr = ~packet.Data2 & 0x7F;
                    spuDr = ~packet.Data3 & 0x0F;
                    spuSl = packet.Data4 & 0x0F;
                    action |= PendingAction.RebuildSpuEnvelope;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetSustainRate:
                {
                    //
                    // Sets the sustain rate SPU register.
                    //

                    spuSMode = 3;
                    spuSr = ~packet.Data2 & 0x7F;
                    action |= PendingAction.RebuildSpuEnvelope;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetReleaseRate:
                {
                    //
                    // Sets the release rate SPU register.
                    //

                    spuRMode = 3;
                    spuRr = ~packet.Data2 & 0x1F;
                    action |= PendingAction.RebuildSpuEnvelope;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetPanning:
                {
                    //
                    // Sets the stereo panning.
                    //

                    panningEnvelope = null;
                    disableDefaultPan = packet.Data2 != 0;
                    currentPanning = Math.Clamp(0xF - (packet.Data3 & 0xF), 0, 14) / 14f;
                    continue;
                }
                case PsxMgsSoundTablePacketType.AutomatePanning:
                {
                    //
                    // Starts a panning envelope.
                    //

                    panningEnvelope = new Envelope(
                        new Vector2(0, currentPanning),
                        new Vector2(packet.Data2 * resolutionMs, Math.Clamp(0xF - (packet.Data3 & 0xF), 0, 14) / 14f)
                    );
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetCoarseTune:
                {
                    //
                    // Sets the number of semitones to adjust played notes by.
                    //

                    coarseTune = unchecked((sbyte)packet.Data2);
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetFineTune:
                {
                    //
                    // Sets the detune in 1/128ths of a semitone.
                    //

                    fineTune = unchecked((sbyte)packet.Data2);
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetPortamentoTime:
                {
                    //
                    // Sets the portamento time.
                    //

                    portamentoTime = packet.Data2 * resolutionMs;
                    continue;
                }
                case PsxMgsSoundTablePacketType.NoteOffAndDelay:
                {
                    ReleaseSpuEnvelope(spuVolumeEnvelope);
                    delayMs += packet.Data2 * resolutionMs;
                    break;
                }
                case PsxMgsSoundTablePacketType.Delay:
                {
                    //
                    // Delays processing the next script packet.
                    //

                    delayMs += packet.Data2 * resolutionMs;
                    break;
                }
                case PsxMgsSoundTablePacketType.End:
                {
                    //
                    // Indicates the end of script packets.
                    //

                    finished = true;
                    break;
                }
                case PsxMgsSoundTablePacketType.SetEOff:
                {
                    //
                    // Disable audio effects.
                    //

                    effectsEnabled = false;
                    continue;
                }
                case PsxMgsSoundTablePacketType.SetEOn:
                {
                    //
                    // Enable audio effects.
                    //

                    effectsEnabled = true;
                    continue;
                }
                case var data1 when (int)data1 >= 0x80:
                {
                    //
                    // Values over 0x80 are script commands; this branch is
                    // executed when the command is not yet supported.
                    //

                    continue;
                }
            }

            //
            // If the sample has changed, convert it.
            //

            if ((action & PendingAction.SetSampleIndex) != 0)
            {
                if (soundBank.FirstOrDefault(x => x.Index == sampleId) is { } sample)
                {
                    sourceSample = vagSplitter
                        .Split(new VagChunk { Channels = 1, Data = sample.Data })
                        .FirstOrDefault();

                    sourceFineTune = unchecked((sbyte)sample.Entry.Tune);
                    sourceTranspose = unchecked((sbyte)sample.Entry.Note);

                    if (!disableDefaultPan)
                        currentPanning = Math.Clamp((int)sample.Entry.Pan, 0, 20) / 20f;

                    spuAr = sample.Entry.AttackRate & 0x7F;
                    spuAMode = sample.Entry.AttackMode & 0b111;
                    spuDr = sample.Entry.DecayRate & 0x0F;
                    spuSMode = sample.Entry.SustainMode & 0b111;
                    spuSr = sample.Entry.SustainRate & 0x7F;
                    spuRr = sample.Entry.ReleaseRate & 0x1F;
                    spuRMode = sample.Entry.ReleaseMode & 0b111;
                    spuSl = sample.Entry.SustainLevel & 0x0F;

                    sampleLoopStart = (int?)sourceSample?[NumericData.LoopStart] ?? -1;
                    sampleLoopEnd = (int?)sourceSample?[NumericData.LoopLength] is { } loopLength &&
                                    sampleLoopStart >= 0
                        ? sampleLoopStart + loopLength
                        : -1;
                }
                else
                {
                    sourceSample = null;
                    sourceFineTune = 0;
                    sourceTranspose = 0;
                    currentPanning = 0.5f;
                    currentVolume = 1f;
                    sampleLoopStart = -1;
                    sampleLoopEnd = -1;
                }

                sourceSampleOffset = 0;
                action &= ~PendingAction.SetSampleIndex;
                action |= PendingAction.RebuildSpuEnvelope;
            }

            //
            // If the SPU envelope needs to be rebuilt, do so.
            //

            if ((action & PendingAction.RebuildSpuEnvelope) != 0)
            {
                if (spuAr >= 0x80)
                    spuAr = 0x7F;

                var attackStep = spuAr & 0b00000011;
                const int attackDir = -1;
                var attackExp = (spuAMode & 0b111) == 0b101;
                var attackShift = (spuAr & 0b01111100) >> 2;

                const int decayStep = 0;
                var decayShift = spuDr & 0b00001111;
                const int decayDir = -1;

                var sustainLevel = spuSl & 0b00001111;

                if (spuSr >= 0x80)
                    spuSr = 0x7F;

                var sustainStep = spuSr & 0b00000011;
                var sustainShift = (spuSr & 0b01111100) >> 2;
                var (sustainDir, sustainExp) = (spuSMode & 0b111) switch
                {
                    0b000 => (-1, 0),
                    0b001 => (1, 0),
                    0b010 or 0b011 or 0b100 => (-1, 0),
                    0b101 => (1, 1),
                    0b110 => (-1, 0),
                    0b111 => (-1, 1)
                };

                if (spuRr >= 0x1F)
                    spuRr = 0x1F;

                var releaseShift = spuRr & 0b00011111;
                const int releaseStep = 0;
                const int releaseDir = -1;
                var releaseExp = (spuRMode & 0b111) == 0b111;

                var sustainLevelFloat = sustainLevel / 15f;
                var attackPoint = CalculateSpuPoint(attackShift, attackStep, attackDir, 0, 1);
                var decayPoint = CalculateSpuPoint(decayShift, decayStep, decayDir, 1, sustainLevelFloat);
                var sustainPoint = CalculateSpuPoint(sustainShift, sustainStep, sustainDir, sustainLevelFloat,
                    sustainLevelFloat);
                var releasePoint = CalculateSpuPoint(releaseShift, releaseStep, releaseDir, sustainLevelFloat, 0);

                spuVolumeEnvelope = CalculateSpuEnvelope(
                    attackPoint.X, decayPoint.X, sustainPoint, releasePoint.X
                );

                action &= ~PendingAction.RebuildSpuEnvelope;
            }

            //
            // Determine how many wave samples will be generated.
            //

            var sampleCount = (int)MathF.Truncate(sampleRate * delayMs / 1000);
            using var buffer0 = MemoryPool<float>.Shared.Rent(sampleCount);
            var sampleFloats0 = buffer0.Memory.Span[..sampleCount];
            using var buffer1 = MemoryPool<float>.Shared.Rent(sampleCount);
            var sampleFloats1 = buffer1.Memory.Span[..sampleCount];
            var sourceSampleSpan = sourceSample != null ? sourceSample.Data.Span : [];
            var timePerSample = 1000f / sampleRate;

            //
            // Process any pending silence.
            //
            
            if (pendingSilence > 0)
            {
                using var silenceBuffer = MemoryPool<float>.Shared.Rent(pendingSilence);
                silenceBuffer.Memory.Span[..pendingSilence].Clear();
                var silenceSpan = silenceBuffer.Memory.Span[..pendingSilence];
                sampleL.Append(silenceSpan);
                sampleR.Append(silenceSpan);
                    
                var silenceMs = pendingSilence * timePerSample;
                    
                if (volumeEnvelope != null)
                    currentVolume = volumeEnvelope.Process(silenceMs);
                    
                if (panningEnvelope != null)
                    currentPanning = panningEnvelope.Process(silenceMs);
                    
                pendingSilence = 0;
            }

            //
            // Populate the output.
            //

            if (sourceSampleOffset < sourceSampleSpan.Length)
            {
                var populatedSampleCount = sampleCount;

                for (var i = 0; i < sampleCount; i++)
                {
                    //
                    // Process envelopes.
                    //

                    var spuVolumeValue = spuVolumeEnvelope?.Process(timePerSample) ?? 1;
                    
                    if (volumeEnvelope != null)
                        currentVolume = volumeEnvelope.Process(timePerSample);
                    
                    if (panningEnvelope != null)
                        currentPanning = panningEnvelope.Process(timePerSample);

                    var sourceSampleOffsetA = (int)MathF.Truncate(sourceSampleOffset);
                    var sourceSampleOffsetB = sourceSampleOffsetA + 1;
                    var weight = sourceSampleOffset - sourceSampleOffsetA;

                    //
                    // If the sample loops, check to see if it needs to loop back.
                    //
                    
                    if (sampleLoopEnd >= 0 && sourceSampleOffsetB >= sampleLoopEnd)
                    {
                        sourceSampleOffset -= sampleLoopEnd - sampleLoopStart;
                        sourceSampleOffsetB = (int)MathF.Truncate(sourceSampleOffset);
                    }

                    //
                    // If we are at the end of the sample, fill the buffer with silence.
                    //

                    else if (sourceSampleOffsetB > sourceSampleSpan.Length)
                    {
                        pendingSilence += sampleCount - i;
                        populatedSampleCount = i;
                        break;
                    }

                    //
                    // Calculate position for linear interpolation.
                    // TODO: Playstation SPU actually uses Gaussian interpolation.
                    //

                    var sourceSample0 = sourceSampleOffsetA < sourceSampleSpan.Length
                        ? sourceSampleSpan[sourceSampleOffsetA]
                        : 0;

                    var sourceSample1 = sourceSampleOffsetB < sourceSampleSpan.Length
                        ? sourceSampleSpan[sourceSampleOffsetB]
                        : 0;

                    //
                    // Calculate interpolated sample value (Lerp function.)
                    //

                    var sourceInterpolatedSample = sourceSample0 + weight * (sourceSample1 - sourceSample0);

                    //
                    // Calculate gain for left/right channels from panning table and volume.
                    //

                    var panIdx = Math.Clamp((int)MathF.Truncate(currentPanning * 40), 0, 40);
                    var panVec = new Vector2(panTbl[40 - panIdx], panTbl[panIdx]) / 127f;
                    var sampleDataVec = panVec * sourceInterpolatedSample * (currentVolume * volumeScale) *
                                        spuVolumeValue;

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
            }

            if (finished)
                break;

            delayMs -= sampleCount * timePerSample;
        }

        result[NumericData.Id] = sampleId;
        return result.ToSound();
    }

    private static Envelope CalculateSpuEnvelope(float attack, float decay, Vector2 sustain, float release)
    {
        const float attackX = 0;
        var decayX = attackX + attack;
        var sustainX = decayX + decay;
        var releaseX = sustainX + sustain.X;
        var endX = releaseX + release;

        return new Envelope(
            new Vector2(0, 0),
            new Vector2(decayX, 1),
            sustain with { X = sustainX },
            sustain with { X = releaseX },
            new Vector2(endX, 0)
        ) { Sustain = 3 };
    }

    private static Vector2 CalculateSpuPoint(int shift, int step, int dir, float level, float target)
    {
        // Not entirely accurate, but good enough for what we're doing.

        if (MathF.Abs(level - target) < float.Epsilon)
            return new Vector2(0, target);

        var actualStep = Math.Abs(dir < 0 ? (step & 0b11) - 8 : 7 - (step & 0b11));
        const float clocksPerSec = 33868800f / 768;
        var newRate = MathF.Pow(2, shift - 11) * actualStep * clocksPerSec;
        var transitionTime = MathF.Abs(target - level) / newRate;
        var result = new Vector2(Math.Max(transitionTime * 1000f, 0), Math.Clamp(target, 0, 1));
        return result;
    }

    private static void AttackSpuEnvelope(Envelope? envelope)
    {
        envelope?.SetPhase(0);
    }

    private static void ReleaseSpuEnvelope(Envelope? envelope)
    {
        envelope?.SetPhase(4);
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
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RhythmCodex.Archs.Psx.Processors;
using RhythmCodex.Games.Mgs.Models;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Helpers;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Games.Mgs.Converters;

[Service]
public sealed class MgsSdSoundScriptRenderer(
    IVagSplitter vagSplitter,
    IAudioDsp audioDsp,
    IMgsSdSoundFrequencyCalculator frequencyCalculator,
    IPsxGaussianInterpolation psxGaussianInterpolation)
    : IMgsSdSoundScriptRenderer
{
    /// <summary>
    /// Panning table used by the Metal Gear Solid sound system.
    /// </summary>
    private static readonly Lazy<ReadOnlyMemory<byte>> PanTbl = new(() => (byte[])
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
        MgsSdSoundScript script,
        List<MgsSdSoundBankEntryWithData> soundBank,
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
        List<MgsSdSoundTablePacket> packets,
        List<MgsSdSoundBankEntryWithData> soundBank,
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
        var panTbl = PanTbl.Value.Span;

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
        Span<float> interpolateBuffer = stackalloc float[4];
        var lastInterpIndex = -1;

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
                    lastInterpIndex = -1;
                    interpolateBuffer.Clear();
                    delayMs += packet.Data2 * resolutionMs;
                    playedNote = packet.Data1;
                    currentVolume = (packet.Data4 & 0x7F) / 127f;
                    sourceSampleRate = frequencyCalculator.Calculate(
                        playedNote,
                        coarseTune + sourceTranspose,
                        fineTune + sourceFineTune
                    );
                    sourceSampleOffset = 0;

                    break;
                }
                case MgsSdSoundTablePacketType.SetTimeResolution:
                {
                    //
                    // Sets the time resolution (how much time each tick represents.)
                    //

                    resolutionMs = MathF.Max(1f, packet.Data2) * 10.4f / 255f;
                    continue;
                }
                case MgsSdSoundTablePacketType.AutomateTimeResolution:
                {
                    //
                    // TODO: I don't know how to go about implementing this command
                    // so the changes will be instantaneous for now.
                    //

                    resolutionMs = MathF.Max(1f, packet.Data2) * 10.4f / 255f;
                    continue;
                }
                case MgsSdSoundTablePacketType.SetSoundBankIndex:
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
                case MgsSdSoundTablePacketType.AutomateVolume:
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
                case MgsSdSoundTablePacketType.SetVolumeScale:
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
                case MgsSdSoundTablePacketType.SetAttackDecay:
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
                case MgsSdSoundTablePacketType.SetSustainRate:
                {
                    //
                    // Sets the sustain rate SPU register.
                    //

                    spuSMode = 3;
                    spuSr = ~packet.Data2 & 0x7F;
                    action |= PendingAction.RebuildSpuEnvelope;
                    continue;
                }
                case MgsSdSoundTablePacketType.SetReleaseRate:
                {
                    //
                    // Sets the release rate SPU register.
                    //

                    spuRMode = 3;
                    spuRr = ~packet.Data2 & 0x1F;
                    action |= PendingAction.RebuildSpuEnvelope;
                    continue;
                }
                case MgsSdSoundTablePacketType.SetPanning:
                {
                    //
                    // Sets the stereo panning.
                    //

                    panningEnvelope = null;
                    disableDefaultPan = packet.Data2 != 0;
                    currentPanning = Math.Clamp(0xF - (packet.Data3 & 0xF), 0, 14) / 14f;
                    continue;
                }
                case MgsSdSoundTablePacketType.AutomatePanning:
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
                case MgsSdSoundTablePacketType.SetCoarseTune:
                {
                    //
                    // Sets the number of semitones to adjust played notes by.
                    //

                    coarseTune = unchecked((sbyte)packet.Data2);
                    continue;
                }
                case MgsSdSoundTablePacketType.SetFineTune:
                {
                    //
                    // Sets the detune in 1/128ths of a semitone.
                    //

                    fineTune = unchecked((sbyte)packet.Data2);
                    continue;
                }
                case MgsSdSoundTablePacketType.SetPortamentoTime:
                {
                    //
                    // Sets the portamento time.
                    //

                    portamentoTime = packet.Data2 * resolutionMs;
                    continue;
                }
                case MgsSdSoundTablePacketType.NoteOffAndDelay:
                {
                    ReleaseSpuEnvelope(spuVolumeEnvelope);
                    delayMs += packet.Data2 * resolutionMs;
                    break;
                }
                case MgsSdSoundTablePacketType.Delay:
                {
                    //
                    // Delays processing the next script packet.
                    //

                    delayMs += packet.Data2 * resolutionMs;
                    break;
                }
                case MgsSdSoundTablePacketType.End:
                {
                    //
                    // Indicates the end of script packets.
                    //

                    finished = true;
                    break;
                }
                case MgsSdSoundTablePacketType.SetEOff:
                {
                    //
                    // Disable audio effects.
                    //

                    effectsEnabled = false;
                    continue;
                }
                case MgsSdSoundTablePacketType.SetEOn:
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

                    var interpIndex = (int)MathF.Truncate(sourceSampleOffset);

                    //
                    // If the sample loops, check to see if it needs to loop back.
                    //
                    
                    if (sampleLoopEnd >= 0 && interpIndex >= sampleLoopEnd)
                    {
                        sourceSampleOffset -= sampleLoopEnd - sampleLoopStart;
                        interpIndex = (int)MathF.Truncate(sourceSampleOffset);
                    }

                    //
                    // If we are at the end of the sample, fill the buffer with silence.
                    //

                    else if (interpIndex > sourceSampleSpan.Length)
                    {
                        pendingSilence += sampleCount - i;
                        populatedSampleCount = i;
                        break;
                    }

                    //
                    // Perform sample interpolation.
                    //

                    if (interpIndex != lastInterpIndex)
                    {
                        interpolateBuffer[1..].CopyTo(interpolateBuffer);
                        interpolateBuffer[3] = interpIndex < sourceSampleSpan.Length
                            ? sourceSampleSpan[interpIndex]
                            : 0;
                        lastInterpIndex = interpIndex;
                    }

                    var sourceInterpolatedSample = psxGaussianInterpolation.InterpolateOne(
                        interpolateBuffer[0],
                        interpolateBuffer[1],
                        interpolateBuffer[2],
                        interpolateBuffer[3],
                        sourceSampleOffset - MathF.Truncate(sourceSampleOffset)
                    );

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
        ) { SustainIndex = 3 };
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
}
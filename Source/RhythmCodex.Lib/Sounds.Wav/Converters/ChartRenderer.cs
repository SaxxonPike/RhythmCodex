using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Mixer.Converters;
using RhythmCodex.Sounds.Mixer.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Providers;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

[Service]
public class ChartRenderer(
    IAudioDsp audioDsp,
    IResamplerProvider resamplerProvider,
    IDefaultStereoMixer defaultStereoMixer
) : IChartRenderer
{
    enum ChannelType
    {
        Free,
        Explicit,
        Priority
    }

    public Sound Render(Chart chart, IEnumerable<Sound> sounds, ChartRendererOptions options)
    {
        var priorityChannelsInUse = 0;
        var sampleBankId = chart[NumericData.SampleMap];
        var channels = new List<(ChannelType Type, int Tag, MixState State)>();
        var sampleMap = new Dictionary<(int Player, int Column, bool Scratch), int>();

        var maxPriorityChannels = (int)(chart[NumericData.PriorityChannels] ?? int.MaxValue);

        var eventList = chart.Events.AsCollection();
        if (eventList.Any(ev => ev[NumericData.LinearOffset] == null))
            throw new RhythmCodexException("Can't render without all events having linear offsets.");

        var soundList = new Dictionary<int, Sound>();

        //
        // Determine the samples within the map that will be used.
        //

        foreach (var sound in sounds)
        {
            if (sound[NumericData.Id] is not { } id || sound[NumericData.SampleMap] != sampleBankId)
                continue;

            var idKey = (int)id;
            soundList.TryAdd(idKey, sound);
        }

        //
        // Preprocess the sounds. This is required to use the Mixer.
        //

        foreach (var (key, sound) in soundList.ToList().AsParallel())
        {
            var processedSound = audioDsp.ApplyResampling(
                sound,
                resamplerProvider.GetBest(),
                options.SampleRate
            );

            soundList[key] = processedSound;
        }

        //
        // Begin mixdown.
        //

        using var mixdownLeft = new SampleBuilder();
        using var mixdownRight = new SampleBuilder();

        var lastSample = 0;
        var eventTicks = eventList
            .GroupBy(ev => (BigRational)ev[NumericData.LinearOffset]!)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var tick in eventTicks)
        {
            var nowSample = (int)(tick.Key * options.SampleRate);

            if (nowSample > lastSample)
            {
                Mix(nowSample - lastSample);
                lastSample = nowSample;
            }

            //
            // Process sound change events (i.e. the key sound is changed.)
            //

            foreach (var ev in tick.Where(t => t[NumericData.LoadSound] != null))
                MapSample(ev, ev[NumericData.LoadSound]);

            //
            // Process note events (i.e. the key is actually sounded.)
            //

            foreach (var ev in tick.Where(t => t[FlagData.Note] != null))
            {
                if (options.UseSourceDataForSamples && ev[NumericData.SourceData] != null)
                    MapSample(ev, ev[NumericData.SourceData]);
                PlayMappedSound(ev);
            }

            //
            // Process BGM events.
            //

            foreach (var ev in tick.Where(t => t[NumericData.PlaySound] != null))
                PlayBgmSound(ev);
        }

        //
        // Finish rendering.
        //

        MixRemaining();

        return new Sound
        {
            [NumericData.Rate] = options.SampleRate,
            Samples = options.SwapStereo
                ? [mixdownRight.ToSample(), mixdownLeft.ToSample()]
                : [mixdownLeft.ToSample(), mixdownRight.ToSample()]
        };

        //
        // Extracts column info from event metadata.
        //

        (int Player, int Column, bool Scratch)? GetMapKey(Event? ev)
        {
            if (ev?[NumericData.Player] is not { } player)
                return null;

            var isScratch = ev[FlagData.Scratch] == true || ev[FlagData.FreeZone] == true;

            if (ev[NumericData.Column] is not { } column)
            {
                // Scratches are permitted to have no column metadata.

                if (!isScratch)
                    return null;

                column = 0;
            }

            return ((int)player, (int)column, isScratch);
        }

        //
        // Sets the key sound for a column.
        //

        void MapSample(Event? ev, BigRational? soundIndex)
        {
            if (GetMapKey(ev) is not { } key)
                return;

            if (soundIndex is not { } soundIndexVal)
                sampleMap.Remove(key);
            else
                sampleMap[key] = (int)soundIndexVal;
        }

        //
        // Sets a sound up for playback.
        //

        void PlaySound(Sound? sound, Event? ev)
        {
            if (sound?[NumericData.Channel] is { } soundChannel)
            {
                //
                // Explicit sound channel gets playback replaced.
                //

                var channelIndex = (int)soundChannel;

                for (var i = 0; i < channels.Count; i++)
                {
                    if (channels[i].Type != ChannelType.Explicit || channels[i].Tag != channelIndex)
                        continue;

                    channels.RemoveAt(i);
                    break;
                }

                channels.Add((ChannelType.Explicit, channelIndex, new MixState
                {
                    Sound = sound,
                    EventData = ev
                }));
            }
            else if (sound?[NumericData.Priority] is { } soundPriority)
            {
                //
                // Priority sound channel attempts to find another sound equal or lower priority number to replace.
                //

                var priorityValue = (int)soundPriority;
                
                if (priorityChannelsInUse >= maxPriorityChannels)
                {
                    for (var i = 0; i < channels.Count; i++)
                    {
                        if (channels[i].Type != ChannelType.Priority || channels[i].Tag > priorityValue) 
                            continue;

                        channels.RemoveAt(i);
                        priorityChannelsInUse--;
                        break;
                    }

                    if (priorityChannelsInUse >= maxPriorityChannels)
                        return;
                }

                priorityChannelsInUse++;
                channels.Add((ChannelType.Priority, priorityValue, new MixState
                {
                    Sound = sound,
                    EventData = ev
                }));
            }
            else
            {
                //
                // Free channels always play.
                //

                channels.Add((ChannelType.Free, 0, new MixState
                {
                    Sound = sound,
                    EventData = ev
                }));
            }
        }

        //
        // Sets a sound up for playback. Column data from the event
        // is used to determine which sound to play.
        //

        void PlayMappedSound(Event ev)
        {
            if (GetMapKey(ev) is not { } key)
                return;

            Sound? sound = null;
            if (sampleMap.TryGetValue(key, out var sample))
                soundList.TryGetValue(sample, out sound);

            PlaySound(sound, ev);
        }

        //
        // Sets a sound up for playback. A value from the event is
        // directly used to determine which sound to play.
        //

        void PlayBgmSound(Event ev)
        {
            Sound? sound = null;
            if (ev[NumericData.PlaySound] is { } soundIndex)
                soundList.TryGetValue((int)soundIndex, out sound);

            PlaySound(sound, ev);
        }

        //
        // Mix all currently playing sounds until there is no more data.
        //

        int MixRemaining()
        {
            var max = channels
                .Select(x => x.State.GetMaxLength())
                .DefaultIfEmpty(0)
                .Max();

            return Mix(max);
        }

        //
        // Mix all currently playing sounds.
        //

        int Mix(int maxMixSize)
        {
            if (maxMixSize < 1)
                return 0;

            var maxMixed = 0;
            using var leftMem = MemoryPool<float>.Shared.Rent(maxMixSize);
            using var rightMem = MemoryPool<float>.Shared.Rent(maxMixSize);

            var leftSpan = leftMem.Memory.Span[..maxMixSize];
            var rightSpan = rightMem.Memory.Span[..maxMixSize];

            //
            // Buffers must be cleared because this memory is recycled.
            //

            leftSpan.Clear();
            rightSpan.Clear();

            //
            // Render the fixed channels.
            //

            for (var i = 0; i < channels.Count; i++)
            {
                var (type, tag, state) = channels[i];

                if (state.Sound?.Mixer?.Invoke() is not { } mixer)
                    mixer = defaultStereoMixer;

                var (newState, mixed) = mixer.Mix(leftSpan, rightSpan, state);

                if (mixed > 0)
                {
                    channels[i] = (type, tag, newState);
                }
                else
                {
                    channels.RemoveAt(i--);
                    if (type == ChannelType.Priority)
                        priorityChannelsInUse--;
                }

                maxMixed = Math.Max(maxMixed, mixed);
            }

            mixdownLeft.Append(leftSpan);
            mixdownRight.Append(rightSpan);

            return maxMixSize;
        }
    }
}
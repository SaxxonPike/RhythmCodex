using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Providers;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

[Service]
public class ChartRenderer(IAudioDsp audioDsp, IResamplerProvider resamplerProvider)
    : IChartRenderer
{
    private const float SqrtHalf = 0.70710677f;

    private class ChannelState
    {
        public int? Channel { get; set; }
        public Sound? Sound { get; set; }
        public int Offset { get; set; }
        public int LeftLength { get; set; }
        public int RightLength { get; set; }
        public float LeftToLeftVolume { get; set; }
        public float LeftToRightVolume { get; set; }
        public float RightToLeftVolume { get; set; }
        public float RightToRightVolume { get; set; }
        public bool Playing { get; set; }
    }

    public Sound Render(Chart chart, IEnumerable<Sound> sounds, ChartRendererOptions options)
    {
        var sampleBankId = chart[NumericData.SampleMap];

        var state = new List<ChannelState>();
        var sampleMap = new Dictionary<(int Player, int Column), int>();
        var masterVolume = (float)(options.Volume ?? BigRational.One);

        var eventList = chart.Events.AsCollection();
        if (eventList.Any(ev => ev[NumericData.LinearOffset] == null))
            throw new RhythmCodexException("Can't render without all events having linear offsets.");

        var soundList = new Dictionary<int, Sound>();

        foreach (var sound in sounds)
        {
            if (sound[NumericData.Id] is not { } id || sound[NumericData.SampleMap] != sampleBankId)
                continue;

            var idKey = (int)id;
            if (soundList.ContainsKey(idKey))
                continue;

            var processedSound = audioDsp.ApplyResampling(
                audioDsp.ApplyEffects(sound),
                resamplerProvider.GetBest(),
                options.SampleRate
            );

            soundList.Add(idKey, processedSound);
        }

        var mixdownLeft = new List<float>();
        var mixdownRight = new List<float>();

        var lastSample = 0;
        var eventTicks = eventList
            .GroupBy(ev => (BigRational)ev[NumericData.LinearOffset]!)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var tick in eventTicks)
        {
            var nowSample = (int)(tick.Key * options.SampleRate);
            var tickEvents = tick.ToArray();

            for (; lastSample < nowSample; lastSample++)
                Mix();

            foreach (var ev in tickEvents.Where(t => t[NumericData.LoadSound] != null))
            {
                var column = ev[NumericData.Column] ?? BigRational.Zero;
                if (ev[FlagData.Scratch] == true || ev[FlagData.FreeZone] == true)
                    column += 1000;
                MapSample(ev[NumericData.Player], column, ev[NumericData.LoadSound]);
            }

            foreach (var ev in tickEvents.Where(t => t[FlagData.Note] != null))
            {
                var column = ev[NumericData.Column] ?? BigRational.Zero;
                if (ev[FlagData.Scratch] == true || ev[FlagData.FreeZone] == true)
                    column += 1000;
                if (options.UseSourceDataForSamples && ev[NumericData.SourceData] != null)
                    MapSample(ev[NumericData.Player], column, ev[NumericData.SourceData]);
                StartUserSample(ev[NumericData.Player], column);
            }

            foreach (var ev in tick.Where(t => t[NumericData.PlaySound] != null))
            {
                StartBgmSample(ev[NumericData.PlaySound], ev[NumericData.Panning]);
            }

            state.RemoveAll(s => !s.Playing);
        }

        while (state.Any(s => s.Playing))
            Mix();

        return new Sound
        {
            [NumericData.Rate] = options.SampleRate,
            Samples =
            [
                new Sample { Data = mixdownLeft.ToArray() },
                new Sample { Data = mixdownRight.ToArray() }
            ]
        };

        void MapSample(BigRational? player, BigRational? column, BigRational? soundIndex)
        {
            var playerInt = (int)(player ?? -1);
            var columnInt = (int)(column ?? -1);
            var soundInt = (int)(soundIndex ?? -1);

            if (soundInt >= 0)
                sampleMap[(playerInt, columnInt)] = soundInt;
            else
                sampleMap.Remove((playerInt, columnInt));
        }

        ChannelState SetupSoundChannel(Sound? sound)
        {
            ChannelState? st = null;
            var limit = Math.Max(1, (int)(sound?[NumericData.SimultaneousSounds] ?? 1));

            if (sound?[NumericData.Channel] is { } channel)
            {
                var channelInt = (int)channel;

                if (state.Count(x => x.Channel == channelInt) >= limit)
                {
                    st = state.First(x => x.Channel == channelInt);
                    state.Remove(st);
                    state.Add(st);
                }

                if (st != null)
                    return st;

                st = new ChannelState { Channel = (int)channel };
            }
            else
            {
                if (state.Count(x => x.Sound == sound) >= limit)
                {
                    st = state.First(x => x.Sound == sound);
                    state.Remove(st);
                    state.Add(st);
                }

                if (st != null)
                    return st;

                st = new ChannelState();
            }

            state.Add(st);
            return st;
        }

        void StartUserSample(BigRational? player, BigRational? column)
        {
            var playerInt = (int)(player ?? -1);
            var columnInt = (int)(column ?? -1);

            Sound? sound = null;
            if (sampleMap.TryGetValue((playerInt, columnInt), out var sample))
                soundList.TryGetValue(sample, out sound);

            var st = SetupSoundChannel(sound);

            st.Sound = sound;
            st.Offset = 0;
            st.LeftLength = sound?.Samples[0].Data.Length ?? 0;
            st.RightLength = sound?.Samples[1].Data.Length ?? 0;
            st.LeftToLeftVolume = st.RightToRightVolume = SqrtHalf * masterVolume;
            st.RightToLeftVolume = st.LeftToRightVolume = 0;
            st.Playing = true;
        }

        void StartBgmSample(BigRational? soundIndex, BigRational? panning = null)
        {
            Sound? sound = null;
            if (soundIndex.HasValue)
                soundList.TryGetValue((int)soundIndex.Value, out sound);

            var panningValue = (float)(panning ?? BigRational.OneHalf);
            float rightVolume, leftVolume;

            if (options.LinearPanning)
            {
                rightVolume = Math.Clamp(panningValue, 0f, 1f) * 2;
                leftVolume = (1 - Math.Clamp(panningValue, 0f, 1f)) * 2;
            }
            else
            {
                rightVolume = Math.Clamp(MathF.Sqrt(panningValue), 0f, 1f);
                leftVolume = Math.Clamp(MathF.Sqrt(1f - panningValue), 0f, 1f);
            }

            var st = SetupSoundChannel(sound);

            st.Sound = sound;
            st.Offset = 0;

            if (sound is { Samples.Count: >= 2 })
            {
                st.LeftLength = sound.Samples[0].Data.Length;
                st.RightLength = sound.Samples[1].Data.Length;
            }
            else
            {
                st.LeftLength = 0;
                st.RightLength = 0;
            }

            st.LeftToLeftVolume = leftVolume * masterVolume;
            st.LeftToRightVolume = (1 - leftVolume) * masterVolume;
            st.RightToLeftVolume = (1 - rightVolume) * masterVolume;
            st.RightToRightVolume = rightVolume * masterVolume;
            st.Playing = true;
        }

        void Mix()
        {
            var finalMixLeft = 0f;
            var finalMixRight = 0f;
            foreach (var ch in state)
            {
                if (!ch.Playing)
                    continue;

                if (ch.Sound == null || (ch.Offset >= ch.LeftLength && ch.Offset >= ch.RightLength))
                {
                    ch.Playing = false;
                    continue;
                }

                var left = ch.Sound.Samples[0].Data.Span;
                var right = ch.Sound.Samples[1].Data.Span;

                if (ch.Offset < ch.LeftLength)
                {
                    finalMixLeft += left[ch.Offset] * ch.LeftToLeftVolume;
                    finalMixRight += left[ch.Offset] * ch.LeftToRightVolume;
                }

                if (ch.Offset < ch.RightLength)
                {
                    finalMixLeft += right[ch.Offset] * ch.RightToLeftVolume;
                    finalMixRight += right[ch.Offset] * ch.RightToRightVolume;
                }

                ch.Offset++;
            }

            mixdownLeft.Add(finalMixLeft);
            mixdownRight.Add(finalMixRight);
        }
    }
}
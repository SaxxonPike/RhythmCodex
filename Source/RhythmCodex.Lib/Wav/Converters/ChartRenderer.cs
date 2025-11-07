using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Providers;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

[Service]
public class ChartRenderer(IAudioDsp audioDsp, IResamplerProvider resamplerProvider)
    : IChartRenderer
{
    private const float SqrtHalf = 0.70710677f;
    
    private class ChannelState
    {
        public BigRational? Channel { get; set; }
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

    private class SampleMapping
    {
        public BigRational? Player { get; set; }
        public BigRational? Column { get; set; }
        public BigRational? SoundIndex { get; set; }
    }

    public Sound Render(IEnumerable<Event> events, IEnumerable<Sound> sounds, ChartRendererOptions options)
    {
        var state = new List<ChannelState>();
        var sampleMap = new List<SampleMapping>();
        var masterVolume = (float)(options.Volume ?? BigRational.One);

        var eventList = events.AsCollection();
        if (eventList.Any(ev => ev[NumericData.LinearOffset] == null))
            throw new RhythmCodexException("Can't render without all events having linear offsets.");

        var soundList = sounds
            .Select(s =>
                audioDsp.ApplyResampling(audioDsp.ApplyEffects(s), resamplerProvider.GetBest(), options.SampleRate))
            .ToArray();

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
            var sample = sampleMap.FirstOrDefault(st => st.Player == player && st.Column == column);
            if (sample == null)
            {
                sample = new SampleMapping();
                sampleMap.Add(sample);
            }

            sample.Player = player;
            sample.Column = column;
            sample.SoundIndex = soundIndex;
        }

        void StartUserSample(BigRational? player, BigRational? column)
        {
            var sample = sampleMap.FirstOrDefault(sm => sm.Player == player && sm.Column == column);
            Sound? sound = null;
            if (sample?.SoundIndex != null)
                sound = soundList.FirstOrDefault(s => s?[NumericData.Id] == sample.SoundIndex);
            var channel = sound?[NumericData.Channel];
            ChannelState? st = null;
            if (channel >= 0 && channel < 255)
                st = state.FirstOrDefault(s => s.Channel == channel);

            if (st == null)
            {
                st = new ChannelState { Channel = channel };
                state.Add(st);
            }

            st.Sound = sound;
            st.Offset = 0;
            st.LeftLength = sound?.Samples[0].Data.Length ?? 0;
            st.RightLength = sound?.Samples[1].Data.Length ?? 0;
            st.LeftToLeftVolume = st.LeftToRightVolume = st.RightToLeftVolume = st.RightToRightVolume =
                SqrtHalf * masterVolume;
            st.Playing = true;
        }

        void StartBgmSample(BigRational? soundIndex, BigRational? panning = null)
        {
            var sound = soundList.FirstOrDefault(s => s?[NumericData.Id] == soundIndex);
            var channel = sound?[NumericData.Channel];
            var rightVolume = Math.Clamp(MathF.Sqrt((float)(panning ?? BigRational.OneHalf)), 0f, 1f);
            var leftVolume = Math.Clamp(MathF.Sqrt(1f - (float)(panning ?? BigRational.OneHalf)), 0f, 1f);

            ChannelState? st = null;
            if (channel >= 0 && channel < 255)
                st = state.FirstOrDefault(s => s.Channel == channel);

            if (st == null)
            {
                st = new ChannelState { Channel = channel };
                state.Add(st);
            }

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
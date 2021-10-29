using System.Collections.Generic;
using System.Text.Json;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bmson.Model
{
    [Model]
    public class BmsonDocument : JsonWrapper
    {
        public BmsonDocument(JsonElement element = default) 
            : base(element)
        {
        }

        public string Version
        {
            get => Get<string>("version");
            set => Set("version", value);
        }

        public BmsonInfo Info
        {
            get => GetObject<BmsonInfo>("info");
            set => Set("info", value);
        }

        public List<BmsonBarLine> Lines
        {
            get => GetList<BmsonBarLine>("lines");
            set => Set("lines", value);
        }

        public List<BmsonBpmEvent> BpmEvents
        {
            get => GetList<BmsonBpmEvent>("bpm_events");
            set => Set("bpm_events", value);
        }

        public List<BmsonStopEvent> StopEvents
        {
            get => GetList<BmsonStopEvent>("stop_events");
            set => Set("stop_events", value);
        }

        public List<BmsonSoundChannel> SoundChannels
        {
            get => GetList<BmsonSoundChannel>("sound_channels");
            set => Set("sound_channels", value);
        }

        public BmsonBga Bga
        {
            get => GetObject<BmsonBga>("bga");
            set => Set("bga", value);
        }
    }

    [Model]
    public class BmsonBarLine : JsonWrapper
    {
        public BmsonBarLine(JsonElement element = default) 
            : base(element)
        {
        }

        public long Y
        {
            get => Get<long>("y");
            set => Set("y", value);
        }
    }

    [Model]
    public class BmsonSoundChannel : JsonWrapper
    {
        public BmsonSoundChannel(JsonElement element = default) 
            : base(element)
        {
        }

        public string Name
        {
            get => Get<string>("name");
            set => Set("name", value);
        }

        public List<BmsonNote> Notes
        {
            get => GetList<BmsonNote>("notes");
            set => Set("notes", value);
        }
    }

    [Model]
    public class BmsonNote : JsonWrapper
    {
        public BmsonNote(JsonElement element = default) 
            : base(element)
        {
        }

        public object X
        {
            get => Get<object>("x");
            set => Set("x", value);
        }

        public long Y
        {
            get => Get<long>("y");
            set => Set("y", value);
        }

        public long Length
        {
            get => Get<long>("l");
            set => Set("l", value);
        }

        public bool Continuation
        {
            get => Get<bool>("c");
            set => Set("c", value);
        }
    }

    [Model]
    public class BmsonBpmEvent : JsonWrapper
    {
        public BmsonBpmEvent(JsonElement element = default) 
            : base(element)
        {
        }

        public long Y
        {
            get => Get<long>("y");
            set => Set("y", value);
        }

        public double Bpm
        {
            get => Get<double>("bpm");
            set => Set("bpm", value);
        }
    }

    [Model]
    public class BmsonStopEvent : JsonWrapper
    {
        public BmsonStopEvent(JsonElement element = default) 
            : base(element)
        {
        }

        public long Y
        {
            get => Get<long>("y");
            set => Set("y", value);
        }

        public long Duration
        {
            get => Get<long>("duration");
            set => Set("duration", value);
        }
    }

    [Model]
    public class BmsonStopEvent : JsonWrapper
    {
        public BmsonStopEvent(JsonElement element = default) 
            : base(element)
        {
        }

        public List<BmsonBgaHeader> Headers
        {
            get => GetList<BmsonBgaHeader>("bga_header");
            set => Set("bga_header", value);
        }
    }

    [Model]
    public class BmsonBgaHeader : JsonWrapper
    {
        public BmsonBgaHeader(JsonElement element = default) 
            : base(element)
        {
        }
    }

    [Model]
    public class BmsonBgaEvent : JsonWrapper
    {
        public BmsonBgaEvent(JsonElement element = default) 
            : base(element)
        {
        }
    }
}
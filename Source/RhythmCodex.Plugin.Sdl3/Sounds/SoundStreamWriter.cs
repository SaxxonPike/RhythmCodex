using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Streamers;

namespace RhythmCodex.Plugin.Sdl3.Sounds;

[Service]
public class SoundStreamWriter : ISoundStreamWriter
{
    public void Write(Stream stream, Sound sound)
    {
        Init.EnsureLoaded();
    }
}
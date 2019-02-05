using System;
using System.IO;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Beatmania.Streamers
{
    [Service]
    public class BeatmaniaPs2NewKeysoundStreamReader : IBeatmaniaPs2NewKeysoundStreamReader
    {
        private readonly IVagStreamReader _vagStreamReader;

        public BeatmaniaPs2NewKeysoundStreamReader(IVagStreamReader vagStreamReader)
        {
            _vagStreamReader = vagStreamReader;
        }

        public BeatmaniaPs2KeysoundSet Read(Stream stream)
        {
            // TODO: this
            throw new NotImplementedException();
        }
    }
}
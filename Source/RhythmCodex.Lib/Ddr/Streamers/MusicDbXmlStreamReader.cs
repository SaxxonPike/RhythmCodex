using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Streamers
{
    [Service]
    public class MusicDbXmlStreamReader : IMusicDbXmlStreamReader
    {
        public IEnumerable<MusicDbEntry> Read(Stream stream)
        {
            var xml = XDocument.Load(stream);
            var musics = xml.Root?.Elements("music").ToList();
            var result = musics?.Select(Decode).ToList() ?? throw new Exception("Couldn't read musicdb xml.");

            return result;
        }

        private static MusicDbEntry Decode(XElement parent)
        {
            string GetString(string key)
            {
                var element = parent.Elements(key).FirstOrDefault();
                return element?.Value;
            }

            int? GetInt(string key)
            {
                var value = GetString(key);
                if (value == null)
                    return null;
                return int.Parse(value);
            }

            int[] GetInts(string key)
            {
                var values = GetString(key)?
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                return values ?? new int[0];
            }

            return new MusicDbEntry
            {
                Mcode = GetInt("mcode"),
                BaseName = GetString("basename"),
                Title = GetString("title"),
                Artist = GetString("artist"),
                BpmMax = GetInt("bpmmax"),
                BpmMin = GetInt("bpmmin"),
                Series = GetInt("series"),
                BemaniFlag = GetInt("bemaniflag"),
                Lamp = GetInt("lamp"),
                DiffLv = GetInts("diffLv"),
                Limited = GetInt("limited"),
                LimitedCha = GetInt("limited_cha"),
                BgStage = GetInt("bgstage"),
                GenreFlag = GetInt("genreflag"),
                EventNo = GetInt("eventno"),
                Movie = GetInt("movie"),
                Region = GetInt("region"),
                MovieOffset = GetInt("movieoffset"),
                TitleYomi = GetString("title_yomi")
            };
        }
    }
}
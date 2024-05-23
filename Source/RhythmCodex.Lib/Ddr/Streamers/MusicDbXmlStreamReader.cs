using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Streamers;

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
        return new MusicDbEntry
        {
            Mcode = parent.GetInt("mcode"),
            BaseName = parent.GetString("basename"),
            Title = parent.GetString("title"),
            Artist = parent.GetString("artist"),
            BpmMax = parent.GetInt("bpmmax"),
            BpmMin = parent.GetInt("bpmmin"),
            Series = parent.GetInt("series"),
            BemaniFlag = parent.GetInt("bemaniflag"),
            Lamp = parent.GetInt("lamp"),
            DiffLv = parent.GetInts("diffLv"),
            Limited = parent.GetInt("limited"),
            LimitedCha = parent.GetInt("limited_cha"),
            BgStage = parent.GetInt("bgstage"),
            GenreFlag = parent.GetInt("genreflag"),
            EventNo = parent.GetInt("eventno"),
            Movie = parent.GetInt("movie"),
            Region = parent.GetInt("region"),
            MovieOffset = parent.GetInt("movieoffset"),
            TitleYomi = parent.GetString("title_yomi")
        };
    }
}
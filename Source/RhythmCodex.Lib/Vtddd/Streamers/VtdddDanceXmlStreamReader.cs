using System.IO;
using System.Linq;
using System.Xml.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Streamers;

[Service]
public class VtdddDanceXmlStreamReader : IVtdddDanceXmlStreamReader
{
    public VtdddDanceDb Read(Stream stream, string chartPrefix)
    {
        var doc = XDocument.Load(stream);
        var songNodes = doc.Root?.Elements("tracks").SelectMany(e => e.Elements()) ?? [];
        var songDb = songNodes.Select((e, i) => DecodeSong(e, i, chartPrefix)).ToList();
        return new VtdddDanceDb
        {
            Tracks = songDb
        };
    }

    private static VtdddDanceDbSong DecodeSong(XElement node, int index, string chartPrefix)
    {
        var songId = int.Parse(node.Name.LocalName.Replace("song_", string.Empty));
        return new VtdddDanceDbSong
        {
            Artist = node.GetString("artist"),
            Title = node.GetString("title"),
            DifficultyEasy = node.GetInt("difficulty_easy"),
            DifficultyMedium = node.GetInt("difficulty_medium"),
            DifficultyHard = node.GetInt("difficulty_hard"),
            DifficultyExpert = node.GetInt("difficulty_expert"),
            Silliness = node.GetInt("silliness"),
            BpmEasy = node.GetInt("bpm_easy"),
            BpmMedium = node.GetInt("bpm_medium"),
            BpmHard = node.GetInt("bpm_hard"),
            AlbumId = node.GetInt("albumID"),
            Wave = node.GetString("wave"),
            Lyrics = node.GetString("lyrics"),
            SongId = songId,
            ChartEasy = $"{chartPrefix}{songId:D2}_0.xml",
            ChartMedium = $"{chartPrefix}{songId:D2}_1.xml",
            ChartHard = $"{chartPrefix}{songId:D2}_2.xml",
            ChartExpert = $"{chartPrefix}{songId:D2}_3.xml"
        };
    }
}
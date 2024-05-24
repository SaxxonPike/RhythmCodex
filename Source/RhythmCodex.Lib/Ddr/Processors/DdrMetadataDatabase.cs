using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Processors;

[Service]
public class DdrMetadataDatabase : IDdrMetadataDatabase
{
    private readonly Lazy<DdrMetadataDatabaseEntry[]> _entries = new(Load);

    private static DdrMetadataDatabaseEntry[] Load()
    {
        var db = EmbeddedResources
            .GetArchive("RhythmCodex.Ddr.Processors.DdrMetadata.zip")
            .Single()
            .Value;
                    
        using var mem = new MemoryStream(db); 
        var doc = XDocument.Load(mem);
        var root = doc.Root;

        if (root == null)
            return [];
        
        var songs = root.Elements("Song").ToArray();

        return songs.Select(xml => new DdrMetadataDatabaseEntry
        {
            Id = xml.GetInt("Id"),
            Code = xml.GetString("Code"),
            Title = xml.GetString("Title"),
            Subtitle = xml.GetString("Subtitle"),
            Artist = xml.GetString("Artist"),
            TitleRoman = xml.GetString("TitleRoman"),
            SubtitleRoman = xml.GetString("SubtitleRoman"),
            ArtistRoman = xml.GetString("ArtistRoman"),
            TitleLocal = xml.GetString("TitleLocal"),
            SubtitleLocal = xml.GetString("SubtitleLocal"),
            ArtistLocal = xml.GetString("ArtistLocal"),
        }).ToArray();
    }

    public DdrMetadataDatabaseEntry? GetByCode(string code) => 
        _entries.Value.SingleOrDefault(x => x.Code == code);
        
    public DdrMetadataDatabaseEntry? GetById(int id) => 
        _entries.Value.SingleOrDefault(x => x.Id == id);
}
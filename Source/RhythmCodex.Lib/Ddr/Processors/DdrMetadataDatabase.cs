using System;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Processors
{
    public class DdrMetadataDatabase : IDdrMetadataDatabase
    {
        private Lazy<DdrMetadataDatabaseEntry[]> _entries;
        
        public DdrMetadataDatabase()
        {
            _entries = new Lazy<DdrMetadataDatabaseEntry[]>(() =>
                {
                    var db = EmbeddedResources.Open("RhythmCodex.Ddr.Processors.DdrMetadata.xml");
                    var doc = XDocument.Load(db);
                    throw new NotImplementedException();
                });
        }
        
        public DdrMetadataDatabaseEntry GetById(string id)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IDdrMetadataDatabase
    {
        DdrMetadataDatabaseEntry GetById(string id);
    }
}
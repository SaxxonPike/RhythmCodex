using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Iso.Model;

[Model]
public class IsoVolume
{
    public string SystemIdentifier { get; set; }
    public string VolumeIdentifier { get; set; }
    public int SpaceSize { get; set; }
    public int SetSize { get; set; }
    public int SequenceNumber { get; set; }
    public int LogicalBlockSize { get; set; }
    public int PathTableSize { get; set; }
    public int TypeLPathTableLocation { get; set; }
    public int OptionalTypeLPathTableLocation { get; set; }
    public int TypeMPathTableLocation { get; set; }
    public int OptionalTypeMPathTableLocation { get; set; }
    public IsoDirectoryRecord? RootDirectoryRecord { get; set; }
    public string VolumeSetIdentifier { get; set; }
    public string PublisherIdentifier { get; set; }
    public string DataPreparerIdentifier { get; set; }
    public string ApplicationIdentifier { get; set; }
    public string CopyrightFileIdentifier { get; set; }
    public string AbstractFileIdentifier { get; set; }
    public string BibliographicFileIdentifier { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ModificationDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public DateTimeOffset EffectiveDate { get; set; }
    public int FileStructureVersion { get; set; }
    public byte[] ApplicationData { get; set; }
}
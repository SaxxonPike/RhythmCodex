using RhythmCodex.Infrastructure;

namespace RhythmCodex.Metadatas.Models;

public interface IMetadata
{
    bool? this[FlagData type] { get; set; }
    BigRational? this[NumericData type] { get; set; }
    string? this[string key] { get; set; }
    string? this[StringData type] { get; set; }
    void CopyTo(IMetadata other);
}
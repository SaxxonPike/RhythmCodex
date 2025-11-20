using JetBrains.Annotations;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Metadatas.Models;

[PublicAPI]
public interface IMetadata
{
    /// <summary>
    /// Gets or sets known boolean metadata values.
    /// </summary>
    bool? this[FlagData type] { get; set; }

    /// <summary>
    /// Gets or sets known numeric metadata values.
    /// </summary>
    BigRational? this[NumericData type] { get; set; }

    /// <summary>
    /// Gets or sets string metadata values. The key is case-insensitive.
    /// </summary>
    string? this[string key] { get; set; }

    /// <summary>
    /// Gets or sets known string metadata values.
    /// </summary>
    string? this[StringData type] { get; set; }

    /// <summary>
    /// Returns true if the metadata content is empty.
    /// </summary>
    bool IsMetadataEmpty { get; }

    /// <summary>
    /// Returns true if the metadata content is identical to another <see cref="Metadata"/>.
    /// </summary>
    /// <remarks>
    /// Also returns true if the other metadata is null, and this metadata is empty.
    /// </remarks>
    bool MetadataEquals(Metadata? other);

    /// <summary>
    /// Creates a deep clone of metadata values.
    /// </summary>
    Metadata CloneMetadata();

    /// <summary>
    /// Copies metadata values to another metadata.
    /// </summary>
    void CopyMetadataTo(IMetadata other);
}
using System.Collections.Generic;

namespace RhythmCodex.FileSystems.Cd.Model;

public interface ICdSectorCollection : IReadOnlyList<ICdSector>
{
    long Length { get; }
}
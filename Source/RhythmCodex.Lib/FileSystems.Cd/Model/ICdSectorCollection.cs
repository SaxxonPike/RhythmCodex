using System;
using System.Collections.Generic;

namespace RhythmCodex.FileSystems.Cd.Model;

public interface ICdSectorCollection : IReadOnlyList<ICdSector>, IDisposable
{
    long Length { get; }
}
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Infrastructure;

public interface IFileAccessor
{
    bool FileExists(string name);
    Stream OpenRead(string name);
    ExtensionMatchedFile? GetFileNameByExtension(string name, IEnumerable<string> extensions);
}
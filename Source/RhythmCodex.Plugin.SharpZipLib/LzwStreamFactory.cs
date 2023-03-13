using ICSharpCode.SharpZipLib.Lzw;
using RhythmCodex.Compression;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.SharpZipLib;

[Service]
public class LzwStreamFactory : ILzwStreamFactory
{
    public Stream Create(Stream source) => 
        new LzwInputStream(source);
}
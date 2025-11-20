using ICSharpCode.SharpZipLib.BZip2;
using RhythmCodex.Compressions.Bzip2.Processors;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.SharpZipLib;

[Service]
public class Bzip2StreamFactory : IBzip2StreamFactory
{
    public Stream Create(Stream source) => 
        new BZip2InputStream(source);
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Arc.Model;
using RhythmCodex.Arc.Streamers;
using RhythmCodex.IoC;

namespace RhythmCodex.Arc;

/// <inheritdoc cref="IArcService"/>
[Service]
public class ArcService(IServiceProvider services)
    : RhythmCodexServiceBase(services), IArcService
{
    public List<ArcFile> ReadArc(Stream stream) =>
        Svc<IArcStreamReader>().Read(stream).ToList();

    public void WriteArc(Stream stream, IEnumerable<ArcFile> files) =>
        Svc<IArcStreamWriter>().Write(stream, files);
}
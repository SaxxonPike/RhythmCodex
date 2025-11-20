using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

[Service]
public class XwbStreamWriter(
    IXwbDataStreamWriter xwbDataStreamWriter,
    IXwbEntryStreamWriter xwbEntryStreamWriter,
    IXwbHeaderStreamWriter xwbHeaderStreamWriter
    ) : IXwbStreamWriter
{
    public int Write(Stream target, IEnumerable<XwbSound> sounds)
    {
        throw new NotImplementedException();
    }
}
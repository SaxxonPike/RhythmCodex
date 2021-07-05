using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbStreamWriter : IXwbStreamWriter
    {
        private readonly IXwbDataStreamWriter _xwbDataStreamWriter;
        private readonly IXwbEntryStreamWriter _xwbEntryStreamWriter;
        private readonly IXwbHeaderStreamWriter _xwbHeaderStreamWriter;

        public int Write(Stream target, IEnumerable<XwbSound> sounds)
        {
            throw new NotImplementedException();
        }
    }
}
﻿using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public interface ISsqStreamReader
    {
        IList<Chunk> Read(Stream stream);
    }
}
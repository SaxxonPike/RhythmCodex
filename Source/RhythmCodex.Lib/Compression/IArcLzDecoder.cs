﻿using System.IO;

namespace RhythmCodex.Compression
{
    public interface IArcLzDecoder
    {
        byte[] Decode(byte[] source);
    }
}
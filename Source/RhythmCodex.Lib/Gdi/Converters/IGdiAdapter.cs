using System;
using System.Drawing;

namespace RhythmCodex.Gdi.Converters
{
    public interface IGdiAdapter : IDisposable
    {
        Bitmap Bitmap { get; }
    }
}
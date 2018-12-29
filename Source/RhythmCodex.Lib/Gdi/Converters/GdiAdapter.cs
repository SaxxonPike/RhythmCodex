using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RhythmCodex.Gdi.Converters
{
    public class GdiAdapter : IGdiAdapter
    {
        private GCHandle _handle;

        public GdiAdapter(Array data, int width, int height, int stride)
        {
            _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, _handle.AddrOfPinnedObject());
        }

        public Bitmap Bitmap { get; private set; }

        public void Dispose()
        {
            Bitmap?.Dispose();
            Bitmap = null;
            if (_handle.IsAllocated)
                _handle.Free();
        }
    }
}
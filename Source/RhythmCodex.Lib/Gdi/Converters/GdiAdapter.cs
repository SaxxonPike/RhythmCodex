using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RhythmCodex.Gdi.Converters
{
    public class GdiAdapter : IGdiAdapter
    {
        private GCHandle _handle;
        private Bitmap _bitmap;

        public GdiAdapter(Array data, int width, int height, int stride)
        {
            _handle = GCHandle.Alloc(data);
            _bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, _handle.AddrOfPinnedObject());
        }

        public Bitmap Bitmap => _bitmap;

        public void Dispose()
        {
            _bitmap?.Dispose();
            _bitmap = null;
            if (_handle.IsAllocated)
                _handle.Free();
        }
    }
}
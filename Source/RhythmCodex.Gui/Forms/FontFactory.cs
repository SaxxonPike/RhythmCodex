using System;
using System.Drawing;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui.Forms
{
    [Service]
    public class FontFactory : IFontFactory, IDisposable
    {
        private readonly Font _normal;
        private readonly Font _normalDark;
        private readonly Font _large;

        public FontFactory()
        {
            _normal = new Font(FontFamily.GenericSansSerif, 9);
            _normalDark = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
            _large = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        }

        public Font GetNormal() => _normal;

        public Font GetNormalDark() => _normalDark;

        public Font GetLarge() => _large;

        public void Dispose()
        {
            _normal?.Dispose();
            _normalDark?.Dispose();
            _large?.Dispose();
        }
    }
}
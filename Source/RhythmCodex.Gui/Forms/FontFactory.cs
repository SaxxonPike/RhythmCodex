using System;
using System.Drawing;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui.Forms;

[Service]
public class FontFactory : IFontFactory, IDisposable
{
    private readonly Font _normal = new(FontFamily.GenericSansSerif, 9);
    private readonly Font _normalDark = new(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
    private readonly Font _large = new(FontFamily.GenericSansSerif, 12, FontStyle.Bold);

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
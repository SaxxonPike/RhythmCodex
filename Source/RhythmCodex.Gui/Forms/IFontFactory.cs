using System.Drawing;

namespace RhythmCodex.Gui.Forms
{
    public interface IFontFactory
    {
        Font GetNormal();
        Font GetNormalDark();
        Font GetLarge();
    }
}
using RhythmCodex.Graphics.Dds.Models;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Graphics.Dds.Converters;

[Service]
public class DdsBitmapDecoder(IDxtDecoder dxtDecoder, IRawBitmapDecoder rawBitmapDecoder) : IDdsBitmapDecoder
{
    public Bitmap Decode(DdsImage image)
    {
        switch (image.PixelFormat.FourCc)
        {
            case 0x00000000: // uncompressed
                return new Bitmap(image.Width,
                    rawBitmapDecoder.Decode32Bit(image.Data.Span, image.Width, image.Height));

            case 0x31545844: // DXT1
                return new Bitmap(image.Width,
                    dxtDecoder.DecodeDxt1(image.Data.Span, image.Width, image.Height,
                        image.PixelFormat.Flags.HasFlag(DdsPixelFormatFlags.DDPF_ALPHAPIXELS)));

            case 0x33545844: // DXT3
                return new Bitmap(image.Width,
                    dxtDecoder.DecodeDxt3(image.Data.Span, image.Width, image.Height));

            case 0x35545844: // DXT5
                return new Bitmap(image.Width,
                    dxtDecoder.DecodeDxt5(image.Data.Span, image.Width, image.Height));

            default:
                throw new RhythmCodexException($"Unsupported FourCC: 0x{image.PixelFormat.FourCc:X8}");
        }
    }
}
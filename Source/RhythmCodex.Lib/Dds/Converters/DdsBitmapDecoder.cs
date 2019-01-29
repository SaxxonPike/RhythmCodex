using System;
using RhythmCodex.Dds.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Dds.Converters
{
    [Service]
    public class DdsBitmapDecoder : IDdsBitmapDecoder
    {
        private readonly IDxtDecoder _dxtDecoder;
        private readonly IRawBitmapDecoder _rawBitmapDecoder;

        public DdsBitmapDecoder(IDxtDecoder dxtDecoder, IRawBitmapDecoder rawBitmapDecoder)
        {
            _dxtDecoder = dxtDecoder;
            _rawBitmapDecoder = rawBitmapDecoder;
        }

        public RawBitmap Decode(DdsImage image)
        {
            switch (image.PixelFormat.FourCc)
            {
                case 0x00000000: // uncompressed
                    return new RawBitmap
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Data = _rawBitmapDecoder.Decode32Bit(image.Data, image.Width, image.Height)
                    };

                case 0x31545844: // DXT1
                    return new RawBitmap
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Data = _dxtDecoder.DecodeDxt1(image.Data, image.Width, image.Height,
                            image.PixelFormat.Flags.HasFlag(DdsPixelFormatFlags.DDPF_ALPHAPIXELS))
                    };

                case 0x33545844: // DXT3
                    return new RawBitmap
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Data = _dxtDecoder.DecodeDxt3(image.Data, image.Width, image.Height)
                    };

                case 0x35545844: // DXT5
                    return new RawBitmap
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Data = _dxtDecoder.DecodeDxt5(image.Data, image.Width, image.Height)
                    };

                default:
                    throw new RhythmCodexException($"Unsupported FourCC: 0x{image.PixelFormat.FourCc:X8}");
            }
        }
    }
}
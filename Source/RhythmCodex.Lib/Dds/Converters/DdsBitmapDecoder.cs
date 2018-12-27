using System;
using RhythmCodex.Dds.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

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
                        Data = _rawBitmapDecoder.Decode32Bit(image.Data.AsSpan(), image.Width, image.Height)
                    };

                case 0x31545844: // DXT1
                    return new RawBitmap
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Data = _dxtDecoder.DecodeDxt1(image.Data, image.Width, image.Height)
                    };
                
                default:
                    throw new RhythmCodexException($"Unsupported FourCC: 0x{image.PixelFormat.FourCc:X8}");
            }
        }
    }

    public interface IDdsBitmapDecoder
    {
        RawBitmap Decode(DdsImage image);
    }
}
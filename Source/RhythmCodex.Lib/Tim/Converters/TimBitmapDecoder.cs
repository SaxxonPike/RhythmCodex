using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Tim.Models;

namespace RhythmCodex.Tim.Converters
{
    [Service]
    public class TimBitmapDecoder : ITimBitmapDecoder
    {
        private readonly ITimColorDecoder _colorDecoder;
        private readonly ITimDataDecoder _dataDecoder;

        public TimBitmapDecoder(ITimColorDecoder colorDecoder, ITimDataDecoder dataDecoder)
        {
            _colorDecoder = colorDecoder;
            _dataDecoder = dataDecoder;
        }

        public IList<RawBitmap> Decode(TimImage image) =>
            Enumerable.Range(0, image.Cluts.Count).Select(i => Decode(image, i)).ToList();

        public RawBitmap Decode(TimImage image, int clutIndex)
        {
            var data = DecodeData(image);
            ConvertColors(data, image, clutIndex);
            
            return new RawBitmap
            {
                Data = data,
                Height = image.Height,
                Width = data.Length / image.Height
            };
        }

        private void ConvertColors(int[] data, TimImage image, int clutIndex)
        {
            switch (image.ImageType)
            {
                case 0x00000000:
                case 0x00000001:
                case 0x00000008:
                case 0x00000009:
                    var clut = GetClut(image, clutIndex);
                    for (var i = 0; i < data.Length; i++)
                        data[i] = clut[i];
                    break;
                case 0x00000002:
                    for (var i = 0; i < data.Length; i++)
                        data[i] = _colorDecoder.Decode16Bit(data[i]);
                    break;
                case 0x00000003:
                    for (var i = 0; i < data.Length; i++)
                        data[i] = _colorDecoder.Decode24Bit(data[i]);
                    break;
            }
        }

        private int[] GetClut(TimImage image, int clutIndex)
        {
            switch (image.ImageType)
            {
                case 0x00000000:
                case 0x00000001:
                case 0x00000008:
                case 0x00000009:
                    return DecodeClut(image.Cluts[clutIndex]);
                default:
                    return null;
            }            
        }

        private int[] DecodeClut(TimPalette clut) => 
            clut.Entries.Select(i => _colorDecoder.Decode16Bit(i)).ToArray();

        private int[] DecodeData(TimImage image)
        {
            switch (image.ImageType)
            {
                case 0x00000000:
                case 0x00000008:
                    return _dataDecoder.Decode4Bit(image.Data, image.Stride, image.Height);
                case 0x00000001:
                case 0x00000009:
                    return _dataDecoder.Decode8Bit(image.Data, image.Stride, image.Height);
                case 0x00000002:
                    return _dataDecoder.Decode16Bit(image.Data, image.Stride, image.Height);
                case 0x00000003:
                    return _dataDecoder.Decode24Bit(image.Data, image.Stride, image.Height);
                default:
                    throw new RhythmCodexException($"Unrecognized TIM image type {image.ImageType:X8}.");
            }
        }
    }
}
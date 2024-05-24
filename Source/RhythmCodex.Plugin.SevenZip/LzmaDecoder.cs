using System;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Lzma.Converters;

namespace RhythmCodex.Plugin.SevenZip;

[Service]
public class LzmaDecoder : ILzmaDecoder
{
    public Memory<byte> Decode(Stream baseStream, int compressedLength, int decompressedLength,
        ReadOnlySpan<byte> decoderProperties = default)
    {
        var lzma = new global::SevenZip.Compression.LZMA.Decoder();

        if (decoderProperties != null)
            lzma.SetDecoderProperties(decoderProperties.ToArray());

        using var outStream = new MemoryStream();
        lzma.Code(baseStream, outStream, compressedLength, decompressedLength, null);
        outStream.Flush();
        return outStream.ToArray();
    }
}
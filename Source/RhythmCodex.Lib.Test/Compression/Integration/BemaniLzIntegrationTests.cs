using System.IO;
using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Compression.Integration;

[TestFixture]
public class BemaniLzIntegrationTests : BaseIntegrationFixture
{
    [Test]
    public void EncodingAndDecoding_ShouldReturnIdenticalData_WhenDataIsRepetitive()
    {
        var data = Enumerable.Range(0, 32).Select(i => unchecked((byte) (i << 4))).ToArray();
            
        var encoder = Resolve<BemaniLzEncoder>();
        var encoded = encoder.Encode(data);
            
        var decoder = Resolve<BemaniLzDecoder>();
        var decoded = decoder.Decode(new ReadOnlyMemoryStream(encoded));

        decoded.ToArray().ShouldBeEquivalentTo(data);
    }

    [Test]
    public void EncodingAndDecoding_ShouldReturnIdenticalData_WhenDataIsRandom()
    {
        var data = CreateMany<byte>(256).ToArray();
            
        var encoder = Resolve<BemaniLzEncoder>();
        var encoded = encoder.Encode(data);
            
        var decoder = Resolve<BemaniLzDecoder>();
        var decoded = decoder.Decode(new ReadOnlyMemoryStream(encoded));

        decoded.ToArray().ShouldBeEquivalentTo(data);
    }

    [Test]
    public void Decoder_ShouldDecodeComplexObject()
    {
        var data = GetArchiveResource("Compression.BemaniLz.TimTest.zip")
            .Values
            .First();
        var expected = GetArchiveResource("Compression.BemaniLz.TimTest.Expected.zip")
            .Values
            .First();

        var decoder = Resolve<BemaniLzDecoder>();
        var decoded = decoder.Decode(new MemoryStream(data));

        decoded.ToArray().ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void Encode_ShouldEncodeComplexObject()
    {
        var data = GetArchiveResource("Compression.BemaniLz.TimTest.zip")
            .Values
            .First();
        var expected = GetArchiveResource("Compression.BemaniLz.TimTest.Expected.zip")
            .Values
            .First();
        var decoder = Resolve<BemaniLzDecoder>();
        var decoded = decoder.Decode(new MemoryStream(data));
        var encoder = Resolve<BemaniLzEncoder>();
        var encoded = encoder.Encode(decoded.Span);
        var reDecoded = decoder.Decode(new ReadOnlyMemoryStream(encoded));

        reDecoded.ToArray().ShouldBeEquivalentTo(expected);
    }
}
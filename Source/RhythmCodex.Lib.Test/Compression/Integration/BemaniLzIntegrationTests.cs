using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Compression.Integration
{
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
            var decoded = decoder.Decode(new MemoryStream(encoded));

            decoded.Should().BeEquivalentTo(data);
        }
    }
}
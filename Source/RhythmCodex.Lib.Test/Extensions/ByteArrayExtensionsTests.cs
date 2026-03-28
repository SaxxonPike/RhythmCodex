using System;
using System.Diagnostics;
using NUnit.Framework;

namespace RhythmCodex.Extensions;

[TestFixture]
public class ByteArrayExtensionsTests : BaseTestFixture
{
    [Test]
    public void Test_Swap16()
    {
        // Arrange.

        var simdBytes = new byte[0x1000000];
        var noSimdBytes = new byte[0x1000000];

        for (var i = 0; i < simdBytes.Length; i++)
            simdBytes[i] = noSimdBytes[i] = unchecked((byte)i);

        // Act.

        var simdSw = new Stopwatch();
        simdSw.Start();
        simdBytes.Swap16();
        simdSw.Stop();

        Log.WriteLine($"Elapsed time for SIMD swap: {simdSw.ElapsedMilliseconds}ms");

        var noSimdSw = new Stopwatch();
        noSimdSw.Start();
        noSimdBytes.Swap16NoSimd();
        noSimdSw.Stop();

        Log.WriteLine($"Elapsed time for non-SIMD swap: {noSimdSw.ElapsedMilliseconds}ms");

        // Assert.

        Assert.That(simdBytes.SequenceEqual(noSimdBytes), Is.True);
        
        for (var i = 0; i < simdBytes.Length; i++)
            Assert.That(simdBytes[i ^ 1], Is.EqualTo(unchecked((byte)i)));
        
        Assert.That(simdSw.ElapsedTicks, Is.LessThan(noSimdSw.ElapsedTicks));
    }
}
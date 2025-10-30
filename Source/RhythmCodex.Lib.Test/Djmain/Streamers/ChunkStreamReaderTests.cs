using System;
using System.IO;
using System.Linq;
using AutoFixture;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Djmain.Heuristics;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers;

[TestFixture]
public class ChunkStreamReaderTests 
    : BaseUnitTestFixture<DjmainChunkStreamReader, IDjmainChunkStreamReader>
{
    private static byte[] GenerateRandomBytes(int length)
    {
        // Note: Given the volume of the data we're using, do not use
        // AutoFixture to generate this data. It'll take forever.
        var rand = new Random();
        var output = new byte[length];
        for (var i = 0; i < length; i++)
            output[i] = unchecked((byte) rand.Next());
        return output;
    }

    private class DjmainHddDescriptionHeuristicStub(Func<byte[], DjmainHddDescription> getter) 
        : IDjmainHddDescriptionHeuristic
    {
        public DjmainHddDescription Get(ReadOnlySpan<byte> chunk) =>
            getter(chunk.ToArray());
    }

    [Test]
    public void Read_ReadsOnlyWholeChunks()
    {
        // Arrange.
        const int chunkSize = DjmainConstants.ChunkSize;
        var input = GenerateRandomBytes(chunkSize * 3 / 2);

        var heuristic = Build<DjmainHddDescription>()
            .With(x => x.BytesAreSwapped, false)
            .Create();
        
        Mocker.Implement<IDjmainHddDescriptionHeuristic>(
            new DjmainHddDescriptionHeuristicStub(_ => heuristic));

        // Act.
        using var mem = new MemoryStream(input);
        var output = Subject.Read(mem).ToArray();
        output.Length.ShouldBe(1, "only whole chunks should be returned");

        // Assert.
        foreach (var chunk in output)
        {
            var data = chunk.Data.Span;
            var offset = chunk.Id * chunkSize;
            for (var i = 0; i < chunkSize; i++, offset++)
            {
                int d0 = data[i];
                int d1 = input[offset];
                if (d0 != d1)
                    throw new Exception($"Expected {d1} but got {d0} at chunk {chunk.Id} index {i:X6}");
            }
        }
    }

    [Test]
    public void Read_ReadsProperChunks()
    {
        // Arrange.
        const int chunkSize = DjmainConstants.ChunkSize;
        var input = GenerateRandomBytes(chunkSize);

        var heuristic = Build<DjmainHddDescription>()
            .With(x => x.BytesAreSwapped, false)
            .Create();

        Mocker.Implement<IDjmainHddDescriptionHeuristic>(
            new DjmainHddDescriptionHeuristicStub(_ => heuristic));
        
        // Act.
        using var mem = new MemoryStream(input);
        var output = Subject.Read(mem);

        // Assert.
        foreach (var chunk in output)
        {
            var data = chunk.Data.Span;
            var offset = chunk.Id * chunkSize;
            for (var i = 0; i < chunkSize; i++, offset++)
            {
                int d0 = data[i];
                int d1 = input[offset];
                if (d0 != d1)
                    throw new Exception($"Expected {d1} but got {d0} at chunk {chunk.Id} index {i:X6}");
            }
        }
    }
}
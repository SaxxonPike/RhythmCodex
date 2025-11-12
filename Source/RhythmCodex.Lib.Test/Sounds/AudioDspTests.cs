using NUnit.Framework;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using Shouldly;

namespace RhythmCodex.Sounds;

[TestFixture]
public class AudioDspTests : BaseUnitTestFixture<AudioDsp>
{
    [Test]
    public void ApplyEffects_AppliesMonoGain()
    {
        var sound = new Sound
        {
            Samples =
            [
                new Sample
                {
                    Data = new float[]
                    {
                        0, 1
                    },
                    [NumericData.Volume] = 0.5f
                }
            ]
        };

        var output = Subject.ApplyEffects(sound);
        
        output.Samples.Count.ShouldBe(2);
        output.Samples[0].Data.ToArray().ShouldBe([0, 0.5f]);
        output.Samples[1].Data.ToArray().ShouldBe([0, 0.5f]);
    }

    [Test]
    public void ApplyEffects_AppliesStereoGain()
    {
        var sound = new Sound
        {
            Samples =
            [
                new Sample
                {
                    Data = new float[]
                    {
                        0, 1
                    },
                    [NumericData.Volume] = 0.5f
                },
                new Sample
                {
                    Data = new float[]
                    {
                        1, 0
                    },
                    [NumericData.Volume] = 0.25f
                }
            ]
        };

        var output = Subject.ApplyEffects(sound);
        
        output.Samples.Count.ShouldBe(2);
        output.Samples[0].Data.ToArray().ShouldBe([0, 0.5f]);
        output.Samples[1].Data.ToArray().ShouldBe([0.25f, 0]);
    }
}
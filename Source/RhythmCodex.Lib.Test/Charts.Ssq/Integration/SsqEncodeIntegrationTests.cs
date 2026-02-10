using NUnit.Framework;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Converters;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Ssq.Integration
{
    [TestFixture]
    public class SsqEncodeIntegrationTests : BaseIntegrationFixture<SsqEncoder>
    {
        [Test]
        public void EncodeSimpleSsq()
        {
            var chart = new Chart
            {
                [StringData.Difficulty] = "Medium",
                Events =
                [
                    new()
                    {
                        [NumericData.MetricOffset] = 0,
                        [NumericData.Bpm] = 120
                    },

                    new()
                    {
                        [NumericData.MetricOffset] = 1,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 0,
                        [NumericData.Player] = 0
                    },

                    new()
                    {
                        [NumericData.MetricOffset] = 1.25,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 1,
                        [NumericData.Player] = 0
                    },

                    new()
                    {
                        [NumericData.MetricOffset] = 1.5,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 2,
                        [NumericData.Player] = 0
                    },

                    new()
                    {
                        [NumericData.MetricOffset] = 1.75,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 3,
                        [NumericData.Player] = 0
                    },

                    new()
                    {
                        [NumericData.MetricOffset] = 2,
                        [FlagData.Note] = true,
                        [NumericData.Column] = 0,
                        [NumericData.Player] = 0
                    }

                ]
            };

            var observed = Subject.Encode([chart]);

            var reversed = Resolve<ISsqDecoder>().Decode(observed);

            // using var stream = this.OpenWrite("ssq\\ssq.bin");
            // Resolve<SsqStreamWriter>().Write(stream, observed);
            // stream.Flush();
        }
    }
}
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Wav.Models
{
    [Model]
    public class MicrosoftAdpcmFormat
    {
        public MicrosoftAdpcmFormat()
        {
        }
        
        public MicrosoftAdpcmFormat(byte[] data)
        {
            SamplesPerBlock = Bitter.ToInt16(data, 2);
            var coefficientCount = Bitter.ToInt16(data, 4);
            Coefficients = new int[coefficientCount * 2];
            for (var i = 0; i < coefficientCount; i++)
                Coefficients[i] = Bitter.ToInt16(data, 6 + i * 2);
        }

        public int[] Coefficients { get; set; }
        public int SamplesPerBlock { get; set; }
    }
}
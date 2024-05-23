using RhythmCodex.Infrastructure;

namespace RhythmCodex.Wav.Models;

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
        for (var i = 0; i < Coefficients.Length; i++)
            Coefficients[i] = Bitter.ToInt16(data, 6 + i * 2);
    }

    public int[] Coefficients { get; set; }
    public int SamplesPerBlock { get; set; }

    public byte[] ToBytes()
    {
        var output = new byte[6 + (Coefficients.Length * 2)];
        var coeffCount = Coefficients.Length / 2;

        output[0x0] = 0x20;
        output[0x1] = 0x00;
        output[0x2] = unchecked((byte) SamplesPerBlock);
        output[0x3] = unchecked((byte) (SamplesPerBlock >> 8));
        output[0x4] = unchecked((byte) coeffCount);
        output[0x5] = unchecked((byte) (coeffCount >> 8));

        for (var i = 0; i < Coefficients.Length; i++)
        {
            output[0x6 + (i << 1)] = unchecked((byte) Coefficients[i]);
            output[0x7 + (i << 1)] = unchecked((byte) (Coefficients[i] >> 8));
        }

        return output;
    }
}
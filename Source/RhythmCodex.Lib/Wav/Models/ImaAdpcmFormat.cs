using RhythmCodex.Infrastructure;

namespace RhythmCodex.Wav.Models;

[Model]
public class ImaAdpcmFormat
{
    public ImaAdpcmFormat(byte[] data)
    {
        SamplesPerBlock = Bitter.ToInt16(data, 2);
    }
        
    public int SamplesPerBlock { get; set; }
}
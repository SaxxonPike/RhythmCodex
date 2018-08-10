namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainSampleInfo
    {
        /// <summary>
        /// Channel number assigned to the sample. 0x1E is "any channel".
        /// </summary>
        byte Channel { get; }
        
        /// <summary>
        /// Sample info register flags (K054539 address: 0x201)
        /// </summary>
        byte Flags { get; }
        
        /// <summary>
        /// Frequency of the sample. Hz = (x * 44100 / 60216)
        /// </summary>
        ushort Frequency { get; }
        
        /// <summary>
        /// Offset within the chunk where the audio starts.
        /// </summary>
        uint Offset { get; }
        
        /// <summary>
        /// Left/right panning value, 0x1-0xF
        /// </summary>
        byte Panning { get; }
        
        /// <summary>
        /// Volume of reverb feedback.
        /// </summary>
        byte ReverbVolume { get; }
        
        /// <summary>
        /// Sample info register flags (K054539 address: 0x200)
        /// </summary>
        byte SampleType { get; }
        
        /// <summary>
        /// Volume of the sample.
        /// </summary>
        byte Volume { get; }
    }
}
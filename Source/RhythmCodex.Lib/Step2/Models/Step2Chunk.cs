using System.Collections.Generic;

namespace RhythmCodex.Step2.Models;

public class Step2Chunk
{
    public List<Step2Metadata> Metadatas { get; set; }
    public byte[] Data { get; set; }
}
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xbox.Model;

[Model]
public class XboxIsoFileEntry
{
    public int StartSector { get; set; }
    public int FileSize { get; set; }
    public XboxIsoFileAttributes Attributes { get; set; }
    public string FileName { get; set; }

    public override string ToString()
    {
        return $"\"{FileName}\" sector={StartSector} size={FileSize} attr={Attributes}";
    }
}
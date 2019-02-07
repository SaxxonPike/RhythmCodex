using System.IO;

namespace RhythmCodex.Iso.Model
{
    public interface ICdFile
    {
        string Name { get; set; }
        Stream Open();
    }
}
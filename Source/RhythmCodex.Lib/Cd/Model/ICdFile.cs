using System.IO;

namespace RhythmCodex.Cd.Model
{
    public interface ICdFile
    {
        string Name { get; set; }
        Stream Open();
    }
}
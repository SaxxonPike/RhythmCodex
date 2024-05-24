using System.Collections.Generic;

namespace RhythmCodex.Step2.Mappers;

public interface IStep2EventMapper
{
    List<int> Map(int panels);
    int Map(IEnumerable<int> panels);
}
using System.Collections.Generic;

namespace RhythmCodex.Cli
{
    public interface ICliModule
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<ICommand> Commands { get; }
    }
}

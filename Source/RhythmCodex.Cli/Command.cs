using System;
using System.Collections.Generic;

namespace RhythmCodex.Cli
{
    public class Command : ICommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Action<IDictionary<string, string[]>> Execute { get; set; }
    }
}

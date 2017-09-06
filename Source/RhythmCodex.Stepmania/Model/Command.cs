using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Stepmania.Model
{
    [Model]
    public class Command
    {
        public string Name { get; set; }
        public IList<string> Values { get; set; }
    }
}

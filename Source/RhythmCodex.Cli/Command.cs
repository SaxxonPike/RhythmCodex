using System;
using System.Collections.Generic;

namespace RhythmCodex.Cli
{
    /// <inheritdoc />
    public class Command : ICommand
    {
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public IEnumerable<ICommandParameter> Parameters { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public Action<IDictionary<string, string[]>> Execute { get; set; }
    }
}
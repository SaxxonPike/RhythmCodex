using System;
using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;

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
        public Action<Args> Execute { get; set; }
    }
}
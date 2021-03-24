﻿using System;
using System.Collections.Generic;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;

namespace RhythmCodex.Cli
{
    /// <inheritdoc />
    public class Command : ICommand
    {
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public IEnumerable<ICommandParameter> Parameters { get; set; } = Array.Empty<ICommandParameter>();
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public Func<Args, ITask> TaskFactory { get; set; } = args => throw new Exception("Execute is not defined.");
    }
}
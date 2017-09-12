using System.Collections.Generic;

namespace RhythmCodex.Infrastructure
{
    /// <inheritdoc />
    public class DirectoryRoute : IDirectoryRoute
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public IEnumerable<IRoutable> Entries { get; set; }
    }
}
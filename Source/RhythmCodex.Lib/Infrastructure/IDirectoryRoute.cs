using System.Collections.Generic;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    ///     A container of routable elements.
    /// </summary>
    public interface IDirectoryRoute : IRoutable
    {
        /// <summary>
        ///     Routable sub-elements.
        /// </summary>
        IEnumerable<IRoutable> Entries { get; }
    }
}
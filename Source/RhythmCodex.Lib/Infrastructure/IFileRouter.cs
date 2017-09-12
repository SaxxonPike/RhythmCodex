using System.Collections.Generic;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// A router which handles the translation between routable file trees and a file system.
    /// </summary>
    public interface IFileRouter
    {
        /// <summary>
        /// Export a file tree to the filesystem.
        /// </summary>
        void Export(string path, IEnumerable<IRoutable> routables);
        
        /// <summary>
        /// Import a file tree from the filesystem.
        /// </summary>
        IEnumerable<IRoutable> Import(string path);
        
        /// <summary>
        /// Flatten a file tree's structure.
        /// </summary>
        IEnumerable<IFileRoute> Flatten(IEnumerable<IRoutable> routables);
    }
}
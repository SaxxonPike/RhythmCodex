using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhythmCodex
{
    /// <summary>
    /// Indicates that a test or tests should not be run during coverage.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DoNotCoverAttribute : Attribute
    {
    }
}

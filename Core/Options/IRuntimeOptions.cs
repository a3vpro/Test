using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;

namespace VisionNet.Core
{
    /// <summary>
    /// Represents a mutable set of named runtime configuration options exposed through indexed accessors.
    /// </summary>
    public interface IRuntimeOptions : IIndexable<string>, INamed, IDescriptible
    {
        /// <summary>
        /// Gets or sets a value indicating whether the option set is currently considered active for evaluation.
        /// </summary>
        bool Enabled { get; set; }
    }
}

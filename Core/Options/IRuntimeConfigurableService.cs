using System.Collections.Generic;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;

namespace VisionNet.Core
{
    /// <summary>
    /// Represents a service capable of updating runtime configuration across multiple managed components.
    /// </summary>
    public interface IRuntimeConfigurableService
    {
        /// <summary>
        /// Refreshes runtime parameters for each managed component using the supplied option instances.
        /// For every dictionary entry, the key identifies the target component and the value provides the new runtime options applied through the component's refresh pipeline.
        /// </summary>
        /// <param name="updatedOptionsDictionary">
        /// Dictionary mapping unique component identifiers (for example, index or name) to their tailored <see cref="IRuntimeOptions"/> instances.
        /// </param>
        void RefreshAllRuntimeParameters(Dictionary<string, IRuntimeOptions> updatedOptionsDictionary);
    }
}

using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;

namespace VisionNet.Core
{
    public interface IRuntimeOptions : IIndexable<string>, INamed, IDescriptible
    {
        /// <summary>
        /// Obtiene o establece si la instancia está habilitada.
        /// </summary>
        bool Enabled { get; set; }
    }
}

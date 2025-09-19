using VisionNet.Core;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;

namespace VisionNet.Core
{
    public interface IRuntimeConfigurable<TOptions>
    where TOptions : IRuntimeOptions, new()
    {
        void RefreshRuntimeParameters(TOptions options);
    }
}

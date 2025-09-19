using System;
using System.Collections.Generic;
using System.Text;

namespace VisionNet.Core.Abstractions
{
    public interface IReconfigurable<TOptions>
        where TOptions : class, IOptions
    {
        void Reconfigure(TOptions options);
    }
}

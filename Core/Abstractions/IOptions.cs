using System;
using System.Collections.Generic;
using System.Text;
using VisionNet.Core.Comparisons;

namespace VisionNet.Core.Abstractions
{
    public interface IOptions
    {
        /// <summary>
        /// Gets the default.
        /// </summary>
        IOptions Default { get; }
    }
}

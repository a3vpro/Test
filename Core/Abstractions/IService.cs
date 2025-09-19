//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Specifies a startable instance
    /// </summary>
    public interface IService<TOptions> : INamed, IDescriptible, IStartable, ISetupable, ISingleton 
        where TOptions : class
    {
    }
}

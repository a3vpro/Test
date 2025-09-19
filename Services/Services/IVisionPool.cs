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
using System;
using VisionNet.Core.Patterns;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public interface IVisionPool : IPoolCollection<IVisionFunction>
    {
        /// <summary>
        /// Returns a IVisionFunction managed by VisionFunction pool of the "index" type
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>IVisionFunction managed by VisionFunction pool</returns>
        IVisionFunction GetFromPool(string index);
    }
}

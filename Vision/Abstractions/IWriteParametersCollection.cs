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
using System.Collections.Generic;
using VisionNet.Core.SafeObjects;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public interface IWriteParametersCollection : IWriteSafeObjectCollection<string, IParameter, BasicTypeCode>
    {


        /// <summary>
        /// List of all parameters that do not have a direct implementation as a property.
        /// </summary>
        IReadOnlyList<IParameter> NotReservedParameters { get; }

        /// <summary>
        /// List of all parameters that do should be stored for persitence
        /// </summary>
        IReadOnlyList<IParameter> ToMeasurablesParameters { get; }
    }
}

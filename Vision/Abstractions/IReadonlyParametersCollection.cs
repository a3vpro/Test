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
    public interface IReadonlyParametersCollection : IReadonlySafeObjectCollection<string, IReadonlyParameter, BasicTypeCode>
    {
        /// <summary>
        /// Reports whether it is an input or output parameter of a vision function.
        /// </summary>
        ParameterDirection Direction { get; }

        /// <summary>
        /// Name of the vision function it belongs to
        /// </summary>
        string ParentName { get; }

        /// <summary>
        /// List of all parameters that do not have a direct implementation as a property.
        /// </summary>
        IReadOnlyList<IReadonlyParameter> NotReservedReadonlyParameters { get; }

        /// <summary>
        /// List of all parameters that do should be stored for persitence
        /// </summary>
        IReadOnlyList<IReadonlyParameter> ToMeasurablesReadonlyParameters { get; }

        /// <summary>
        /// The parameter is marked as readonly. It does not allow to modify its internal value
        /// </summary>
        bool Readonly { get; }

        /// <summary>
        /// Return the value to an image instance.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        IImage ToImage(string key, IImage defaultValue = null);
    }
}

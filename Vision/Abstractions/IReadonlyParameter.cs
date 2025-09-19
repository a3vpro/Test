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
using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;
using VisionNet.Core.SafeObjects;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public interface IReadonlyParameter: IReadonlySafeObject<BasicTypeCode>, IEntity<string>, INamed, IDescriptible
    {
        /// <summary>
        /// Reference of the parameter name of the external vision tool
        /// </summary>
        string ExternalIndex { get; }

        /// <summary>
        /// Name of the vision function it belongs to
        /// </summary>
        string ParentName { get; }

        /// <summary>
        /// It indicates that the parameter is for input or output values
        /// </summary>
        ParameterDirection Direction { get; }

        /// <summary>
        /// It indicates that the parameter is a fixed value or called at runtime")]
        /// </summary>
        ParameterSource Source { get;  }

        /// <summary>
        /// Indicates the destiny of the parametrer.
        /// </summary>
        ParameterScope Scope { get; }


        /// <summary>
        /// Indicates it is vector of the data type of the parametrer.
        /// </summary>
        bool IsArray { get; }

        /// <summary>
        /// The parameter is marked as readonly. It does not allow to modify its internal value
        /// </summary>
        bool Readonly { get; }

        /// <summary>
        /// The parameter will be marked to be included in results
        /// </summary>
        bool SaveToResult { get; }

        /// <summary>
        /// The parameter will be marked to be included in measurables of the product result
        /// </summary>
        bool IncludeInMeasurable { get; }

        /// <summary>
        /// Return the value to an image instance.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        IImage ToImage(IImage defaultValue = null);
    }
}

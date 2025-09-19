//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 23-06-2023
// Description      : v1.7.0
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.            
//-----------------------------------------------
using System;
using System.Collections.Generic;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public interface IVisionMessage
    {
        /// <summary>
        /// Identification of the location of the system (conveyor, cell, factory...)
        /// </summary>
        string SystemSource { get; }

        /// <summary>
        /// Identification of the piece or product
        /// </summary>
        long Index { get; }

        /// <summary>
        /// List of product features like type, size, color, and other modifiers of the inspection algorithm
        /// </summary>
        List<NamedValue> Features { get; }

        /// <summary>
        /// It identifies the inspection step in case that the inspection of the piece or product has multiple steps
        /// </summary>
        List<NamedValue> Step { get; }

        /// <summary>
        /// Moment of the acquisition
        /// </summary>
        DateTime AcquisitionMoment { get; }

        /// <summary>
        /// The collection of the input imagenes (originals)
        /// </summary>
        IImageCollection SourceImages { get; }

        /// <summary>
        /// Previous visión function name
        /// </summary>
        string PrevVisionFunctionName { get; }

        /// <summary>
        /// The collection of all the parameters returnes from the previous vision function
        /// </summary>
        IMessageParametersCollection PrevResults { get; }
    }
}

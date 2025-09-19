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

namespace VisionNet.Vision.Core
{
    public interface IInputParametersCollection: IParametersCollection
    {
        /// <summary>
        /// The inspection is enabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The previous inspection is successful executed
        /// </summary>
        bool PrevResult { get; }

        /// <summary>
        /// Function to set the previous inspection is successful executed parameter
        /// </summary>
        bool TrySetPrevResult(bool prevResult);
    }
}

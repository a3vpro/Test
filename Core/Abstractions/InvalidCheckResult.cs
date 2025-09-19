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
namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Represents the result of an invalid check, including descriptions of the cause and what is needed to resolve it.
    /// </summary>
    public class InvalidCheckResult
    {
        /// <summary>
        /// Gets or sets the description of the cause of the invalid check.
        /// </summary>
        public string CauseDescription { get; set; }

        /// <summary>
        /// Gets or sets the description of what is needed to resolve the invalid check.
        /// </summary>
        public string NeedDescription { get; set; }

        /// <summary>
        /// Returns a string representation of the invalid check result, combining the cause and need descriptions.
        /// </summary>
        /// <returns>A string that concatenates the <see cref="CauseDescription"/> and <see cref="NeedDescription"/>.</returns>
        public override string ToString()
        {
            return CauseDescription + NeedDescription;
        }
    }

}

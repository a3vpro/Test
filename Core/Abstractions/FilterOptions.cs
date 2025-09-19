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
    /// Specifies the filtering options available for processing collections or datasets.
    /// </summary>
    public enum FilterOption
    {
        /// <summary>
        /// Indicates that all items should be included without filtering.
        /// </summary>
        All,

        /// <summary>
        /// Indicates that only items matching the specified criteria should be included.
        /// </summary>
        Include,

        /// <summary>
        /// Indicates that items matching the specified criteria should be excluded.
        /// </summary>
        Exclude
    }

}

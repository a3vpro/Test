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
    /// <summary>
    /// Defines the criteria used to determine the success of an instance segmentation process
    /// </summary>
    public enum ResultLogic
    {
        /// <summary>
        /// This value indicates that the segmentation process is considered successful 
        /// if it identifies one or more instances within the analyzed data.
        /// A scenario where instances are detected signifies a positive outcome, 
        /// reflecting the effectiveness of the segmentation in finding relevant entities.
        /// </summary>
        GoodIfFound,

        /// <summary>
        /// This value signifies that not finding any instances during the segmentation 
        /// process is deemed a successful outcome.
        /// This could apply in contexts where the absence of instances is the desired result,
        /// suggesting that the analyzed data does not contain any objects of interest (defects),
        /// or that the conditions being monitored are within acceptable parameters
        /// </summary>
        GoodIfNotFound,
    }
}

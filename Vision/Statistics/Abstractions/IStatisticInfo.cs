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

namespace VisionNet.Vision.Core
{
    public interface IStatisticInfo
    {
        //string InspectionName { get; }

        /// <summary>
        /// Gets the information of the statistics
        /// </summary>
        /// <param name="maxDuration">Moment of the statistics</param>
        /// <returns>Information of th estatistics</returns>
        IStatisticInfo Get(TimeSpan maxDuration = default);

        /// <summary>
        /// Total population used in statistics
        /// </summary>
        long Population { get; }

        /// <summary>
        /// Total population that match with the criteria of the statistics
        /// </summary>
        long MatchingCriteria { get; }

        /// <summary>
        /// Rate of population that meets criteria
        /// </summary>
        double? Rate { get; }

        /// <summary>
        /// Moment of last reset
        /// </summary>
        DateTime ResetMoment { get; }

        /// <summary>
        /// Moment of las update of statistics
        /// </summary>
        DateTime LastUpdateMoment { get; }

        /// <summary>
        /// Period of time getting statistics
        /// </summary>
        TimeSpan Duration { get; }
    }
}
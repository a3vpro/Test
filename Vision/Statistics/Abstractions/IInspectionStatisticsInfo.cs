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
using VisionNet.Core.Abstractions;

namespace VisionNet.Vision.Core
{
    public interface IInspectionStatisticsInfo : IStatisticInfo, INamed
    {
        /// <summary>
        /// Global statistics of the inspection
        /// </summary>
        IStatisticInfo Global { get; }

        /// <summary>
        /// Timed statistics of the inspection
        /// </summary>
        IReadOnlyList<IStatisticInfo> Timed { get; }

        /// <summary>
        /// Total population used in statistics
        /// </summary>
        long GetPopulation(TimeSpan maxDuration);

        /// <summary>
        /// Total population that match with the criteria of the statistics
        /// </summary>
        long GetMatchingCriteria(TimeSpan maxDuration);

        /// <summary>
        /// Rate of population that meets criteria
        /// </summary>
        double? GetRate(TimeSpan maxDuration);
    }
}
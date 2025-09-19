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
using System.ComponentModel;

namespace VisionNet.Vision.Core
{
    [Serializable]
    public class SingleStatisticsOptions
    {
        /// <summary>
        /// Total population used in statistics
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(SingleStatisticsOptions))]
        [Description("Total population used in statistics.")]
        [DisplayName(nameof(Population))]
        public long Population { get; set; }

        /// <summary>
        /// Total population that match with the criteria of the statistics
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(SingleStatisticsOptions))]
        [Description("Total population that match with the criteria of the statistics.")]
        [DisplayName(nameof(MatchingCriteria))]
        public long MatchingCriteria { get; set; }

        /// <summary>
        /// Moment of last reset
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(SingleStatisticsOptions))]
        [Description("Moment of last reset.")]
        [DisplayName(nameof(ResetMoment))]
        public DateTime ResetMoment { get; set; } = default;

        /// <summary>
        /// Moment of las update of statistics
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(SingleStatisticsOptions))]
        [Description("Moment of las update of statistics.")]
        [DisplayName(nameof(LastUpdateMoment))]
        public DateTime LastUpdateMoment { get; set; } = default;

        public SingleStatisticsOptions()
        {
            ResetMoment = DateTime.Now;
            LastUpdateMoment = ResetMoment;
        }
    }
}
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
using System.ComponentModel;

namespace VisionNet.Vision.Core
{
    [Serializable]
    public class TimedStatisticsOptions
    {
        /// <summary>
        /// Divided statistics
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(TimedStatisticsOptions))]
        [Description("Raw data of statistics.")]
        [DisplayName(nameof(RawData))]
        public Dictionary<DateTime, SingleStatistic> RawData { get; set; } = new Dictionary<DateTime, SingleStatistic>();

        /// <summary>
        /// Moment of las update of statistics
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(TimedStatisticsOptions))]
        [Description("Moment of las update of statistics.")]
        [DisplayName(nameof(LastUpdateMoment))]
        public DateTime LastUpdateMoment { get; set; } = default;

        /// <summary>
        /// Time interval used to group statistics
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(TimedStatisticsOptions))]
        [Description("Time interval used to group statistics.")]
        [DisplayName(nameof(TimeInterval))]
        public TimeSpan TimeInterval { get; set; }

        /// <summary>
        /// Maximum duration of the statistics
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(TimedStatisticsOptions))]
        [Description("Maximum duration of the statistics.")]
        [DisplayName(nameof(MaxDuration))]
        public TimeSpan MaxDuration { get; set; }

        public TimedStatisticsOptions(TimeSpan timeInterval, TimeSpan maxDuration)
        {
            TimeInterval = timeInterval;
            MaxDuration = maxDuration;
        }

        public TimedStatisticsOptions()
        {
            TimeInterval = TimeSpan.FromMinutes(1);
            MaxDuration = TimeSpan.FromMinutes(5);
        }
    }
}
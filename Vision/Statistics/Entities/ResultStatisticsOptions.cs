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
using VisionNet.Core.Serialization;
using VisionNet.IO.FilePersistence;
using VisionNet.DataBases;
using VisionNet.Core.Abstractions;
using Newtonsoft.Json;

namespace VisionNet.Vision.Core
{
    [Serializable]
    public class ResultStatisticsOptions: OptionsBase
    {
        /// <summary>
        /// Global statistics configuration
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(InspectionStatisticsOptions))]
        [Description("Database connection options.")]
        [DisplayName(nameof(DBConnection))]
        public DBConnectionOptions DBConnection { get; set; } = new DBConnectionOptions();

        /// <summary>
        /// Global statistics configuration
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(InspectionStatisticsOptions))]
        [Description("Global options.")]
        [DisplayName(nameof(Global))]
        public InspectionStatisticsOptions Global { get; set; } = new InspectionStatisticsOptions();

        /// <summary>
        /// Timed statistics configuration
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category(nameof(InspectionStatisticsOptions))]
        [Description("Inspections options.")]
        [DisplayName(nameof(Inspections))]
        public List<InspectionStatisticsOptions> Inspections { get; set; } = new List<InspectionStatisticsOptions>();        

        public ResultStatisticsOptions()
        {
            Backup = false;
        }

        [JsonIgnore]
        [Browsable(false)]
        public override IOptions Default
        {
            get
            {
                var timeInterval = TimeSpan.FromSeconds(60);
                var maxDuration01 = TimeSpan.FromMinutes(1);
                var maxDuration05 = TimeSpan.FromMinutes(5);
                var maxDuration15 = TimeSpan.FromMinutes(15);
                var maxDuration30 = TimeSpan.FromMinutes(30);
                var maxDuration60 = TimeSpan.FromMinutes(60);
                var now = DateTime.Now;

                var newTimedStatisticsOptions = new Func<TimeSpan, TimedStatisticsOptions>((maxDuration) =>
                {
                    return new TimedStatisticsOptions
                    {
                        TimeInterval = timeInterval,
                        MaxDuration = maxDuration,
                        RawData = new Dictionary<DateTime, SingleStatistic>(),
                        LastUpdateMoment = now
                    };
                });

                var newInspectionStatisticsOptions = new Func<string, InspectionStatisticsOptions>(name =>
                {
                    return new InspectionStatisticsOptions()
                    {
                        Name = name,
                        Timed = new List<TimedStatisticsOptions>()
                        {
                            {newTimedStatisticsOptions(maxDuration01)},
                            {newTimedStatisticsOptions(maxDuration05)},
                            {newTimedStatisticsOptions(maxDuration15)},
                            {newTimedStatisticsOptions(maxDuration30)},
                            {newTimedStatisticsOptions(maxDuration60)},
                        }
                    };
                });

                return new ResultStatisticsOptions
                {
                    Global = newInspectionStatisticsOptions("Global"),
                    Inspections = new List<InspectionStatisticsOptions>
                    {
                        //{ newInspectionStatisticsOptions("ForeignObjects") },
                        //{ newInspectionStatisticsOptions("MeatClassifier") },
                    }
                };
            }
        }

        /// <summary>
        /// Gets the default.
        /// </summary>
        public static ResultStatisticsOptions DefaultInstance => new ResultStatisticsOptions().Default as ResultStatisticsOptions;
    }
}
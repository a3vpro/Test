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
using System.Linq;
using Conditions;
using Microsoft.Extensions.Options;
using VisionNet.Core.Exceptions;

namespace VisionNet.Vision.Core
{
    public class TimedStatistic : IStatistic, IConfigureOptions<TimedStatisticsOptions>
    {
        private object _lockObject = new object();

        /// <summary>
        /// Options used tu configure the statistics
        /// </summary>
        public virtual TimedStatisticsOptions Options { get; protected set; } = new TimedStatisticsOptions();

        /// <summary>
        /// Total population used in statistics
        /// </summary>
        public long Population => GetPopulation();
        /// <summary>
        /// Total population used in statistics
        /// </summary>
        public long GetPopulation(DateTime dateTime = default)
        {
            long result = 0;
            lock (_lockObject)
            {
                if (dateTime == default(DateTime))
                    result = Options.RawData.Values.Sum(s => s.Population);
                else if (Options.RawData.ContainsKey(dateTime))
                    result = Options.RawData[dateTime].Population;
            }
            return result;
        }

        /// <summary>
        /// Total population that match with the criteria of the statistics
        /// </summary>
        public long MatchingCriteria => GetMatchingCriteria();
        /// <summary>
        /// Total population that match with the criteria of the statistics
        /// </summary>
        public long GetMatchingCriteria(DateTime dateTime = default)
        {
            long result = 0;
            lock (_lockObject)
            {
                if (dateTime == default(DateTime))
                    result = Options.RawData.Values.Sum(s => s.MatchingCriteria);
                if (Options.RawData.ContainsKey(dateTime))
                    result = Options.RawData[dateTime].MatchingCriteria;
            }
            return result;
        }

        /// <summary>
        /// Rate of population that meets criteria
        /// </summary>
        public double? Rate => GetRate();
        /// <summary>
        /// Rate of population that meets criteria
        /// </summary>
        public double? GetRate(DateTime dateTime = default)
        {
            double? result = 0;
            lock (_lockObject)
            {
                if (dateTime == default(DateTime))
                {
                    var mc = (double?)Options.RawData.Values.Sum(s => s.MatchingCriteria);
                    var p = (double?)Options.RawData.Values.Sum(s => s.Population);
                    result = p <= 0 ? null : mc / p;
                }
                if (Options.RawData.ContainsKey(dateTime))
                    result = Options.RawData[dateTime].Rate;
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime ResetMoment => DateTime.Now - Options.MaxDuration;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastUpdateMoment => Options.LastUpdateMoment;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TimeSpan Duration => DateTime.Now - ResetMoment;

        /// <summary>
        /// Confituration set of the instance
        /// </summary>
        /// <param name="options">Input configuration values</param>
        public virtual void Configure(TimedStatisticsOptions options)
        {
            try
            {
                options.RawData.Requires(nameof(options.RawData)).IsNotNull();

                foreach (var r in options.RawData)
                {
                    r.Value.Population.Requires(nameof(options.RawData)).IsGreaterOrEqual(0);
                    r.Value.MatchingCriteria.Requires(nameof(options.RawData))
                        .IsGreaterOrEqual(0)
                        .IsLessOrEqual(r.Value.Population);
                }

                lock (_lockObject)
                    Options = options;
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationParameterException($"Exception during configuration of {nameof(this.GetType)}", ex);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IStatisticInfo Get(TimeSpan maxDuration = default)
        {
            return this;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Update(bool matchCriteria, DateTime dateTime = default)
        {
            if (dateTime == default)
                dateTime = DateTime.Now;
            Options.LastUpdateMoment = dateTime;
            var truncatedDateTime = new DateTime(Options.TimeInterval.Ticks * (dateTime.Ticks / Options.TimeInterval.Ticks)); // Parte entera

            lock (_lockObject)
            {
                if (!Options.RawData.ContainsKey(truncatedDateTime))
                    Options.RawData[truncatedDateTime] = new SingleStatistic();
                Options.RawData[truncatedDateTime].Update(matchCriteria, truncatedDateTime);
            }

            Refresh(truncatedDateTime);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Reset(DateTime dateTime = default)
        {
            lock (_lockObject)
            {
                Options.RawData.Clear();
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Refresh(DateTime dateTime = default)
        {
            if (dateTime == default)
                dateTime = DateTime.Now;
            lock (_lockObject)
            {
                Options.RawData = Options.RawData.Where(s => s.Key > (dateTime - Options.MaxDuration))
                                     .ToDictionary(pair => pair.Key,
                                                   pair => pair.Value);
            }
        }
    }
}
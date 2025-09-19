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
using Conditions;
using System.Linq;
using Microsoft.Extensions.Options;
using VisionNet.Core.Exceptions;

namespace VisionNet.Vision.Core
{
    public class InspectionStatistics: IInspectionStatistics, IConfigureOptions<InspectionStatisticsOptions>
    {
        private object _lockObject = new object();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public InspectionStatisticsOptions Options { get; private set; } = new InspectionStatisticsOptions();

        private SingleStatistic _global = new SingleStatistic();
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IStatisticInfo Global => _global;

        private List<TimedStatistic> _timed = new List<TimedStatistic>();
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IReadOnlyList<IStatisticInfo> Timed => _timed;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Name => Options.Name;

        public IStatisticInfo Get(TimeSpan maxDuration = default)
        {
            IStatisticInfo result = _global;
            lock (_lockObject)
            {
                var st = _timed.FirstOrDefault(s => s.Options.MaxDuration == maxDuration);
                result = st == null ? _global : st as IStatisticInfo;
            }
            return result;
        }


        /// <summary>
        /// Total population used in statistics
        /// </summary>
        public long Population => _global.Population;
        /// <summary>
        /// Total population used in statistics
        /// </summary>
        public long GetPopulation(TimeSpan maxDuration)
        {
            long result = 0;
            lock (_lockObject)
            {
                var st = _timed.FirstOrDefault(s => s.Options.MaxDuration == maxDuration);
                result = st == null ? Population : st.Population;
            }
            return result;
        }

        /// <summary>
        /// Total population that match with the criteria of the statistics
        /// </summary>
        public long MatchingCriteria => _global.MatchingCriteria;
        /// <summary>
        /// Total population that match with the criteria of the statistics
        /// </summary>
        public long GetMatchingCriteria(TimeSpan maxDuration)
        {
            long result = 0;
            lock (_lockObject)
            {
                var st = _timed.FirstOrDefault(s => s.Options.MaxDuration == maxDuration);
                result = st == null ? MatchingCriteria : st.MatchingCriteria;
            }
            return result;
        }

        /// <summary>
        /// Rate of population that meets criteria
        /// </summary>
        public double? Rate => _global.Rate;
        /// <summary>
        /// Rate of population that meets criteria
        /// </summary>
        public double? GetRate(TimeSpan maxDuration)
        {
            double? result = 0;
            lock (_lockObject)
            {
                var st = _timed.FirstOrDefault(s => s.Options.MaxDuration == maxDuration);
                result = st == null ? Rate : st.Rate;
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime ResetMoment => _global.ResetMoment;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastUpdateMoment => _global.LastUpdateMoment;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TimeSpan Duration => _global.Duration;

        public void Configure(InspectionStatisticsOptions options)
        {
            try
            {
                options.Name.Requires(nameof(options.Name)).IsNotNullOrWhiteSpace();
                options.Timed.Requires(nameof(options.Timed)).IsNotNull();

                _global.Configure(options.Global);

                _timed.Clear();
                foreach (var opt in options.Timed)
                {
                    var timed = new TimedStatistic();
                    timed.Configure(opt);
                    _timed.Add(timed);
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
        public void Refresh(DateTime dateTime = default)
        {
            if (dateTime == default)
                dateTime = DateTime.Now;
            lock (_lockObject)
            {
                _global.Refresh(dateTime);
                foreach (var timed in _timed)
                    timed.Refresh(dateTime);
            }
            RaiseUpdated(this, new EventArgs());
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Reset(DateTime dateTime = default)
        {
            if (dateTime == default)
                dateTime = DateTime.Now;
            lock (_lockObject)
            {
                _global.Reset(dateTime);
                foreach (var timed in _timed)
                    timed.Reset(dateTime);
            }
            RaiseUpdated(this, new EventArgs());
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Update(bool matchCriteria, DateTime dateTime = default)
        {
            if (dateTime == default)
                dateTime = DateTime.Now;
            lock (_lockObject)
            {
                _global.Update(matchCriteria, dateTime);
                foreach (var timed in _timed)
                    timed.Update(matchCriteria, dateTime);
            }
            RaiseUpdated(this, new EventArgs());
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Update(InspectionResult inspectionResult, DateTime dateTime = default)
        {
            Update(inspectionResult.Result, dateTime);
        }

        protected void RaiseUpdated(object sender, EventArgs eventArgs)
        {
            try
            {
                Updated?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseUpdated));
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<EventArgs> Updated;
    }
}
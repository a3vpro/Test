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
using Conditions;
using Microsoft.Extensions.Options;
using VisionNet.Core.Exceptions;

namespace VisionNet.Vision.Core
{
    public class SingleStatistic: IStatistic, IConfigureOptions<SingleStatisticsOptions>
    {
        /// <summary>
        /// Options used tu configure the statistics
        /// </summary>
        public virtual SingleStatisticsOptions Options { get; protected set; } = new SingleStatisticsOptions();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public long Population => Options.Population;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public long MatchingCriteria => Options.MatchingCriteria;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public double? Rate => Population <= 0 ? null : (double?)Options.MatchingCriteria / (double?)Options.Population;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime ResetMoment => Options.ResetMoment;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime LastUpdateMoment => Options.LastUpdateMoment;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TimeSpan Duration => DateTime.Now - Options.ResetMoment;

        /// <summary>
        /// Confituration set of the instance
        /// </summary>
        /// <param name="options">Input configuration values</param>
        public virtual void Configure(SingleStatisticsOptions options)
        {
            try
            {
                options.Population.Requires(nameof(options.Population)).IsGreaterOrEqual(0);
                options.MatchingCriteria.Requires(nameof(options.MatchingCriteria))
                    .IsGreaterOrEqual(0)
                    .IsLessOrEqual(options.Population);
                options.LastUpdateMoment.Requires(nameof(options.LastUpdateMoment)).IsGreaterOrEqual(options.ResetMoment);

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
            Options.Population++;

            if (matchCriteria)
                Options.MatchingCriteria++;

            if (dateTime == default)
                dateTime = DateTime.Now;
            Options.LastUpdateMoment = dateTime;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Reset(DateTime dateTime = default)
        {
            Options.MatchingCriteria = 0;
            Options.Population = 0;

            if (dateTime == default)
                dateTime = DateTime.Now;
            Options.ResetMoment = dateTime;
            Options.LastUpdateMoment = dateTime;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Refresh(DateTime dateTime = default)
        {
        }
    }
}
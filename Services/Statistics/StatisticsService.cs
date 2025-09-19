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
//#define STATISTICS_EMULATION
using System;
using System.Windows.Threading;
using Castle.Core.Logging;
using VisionNet.Application;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Exceptions;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    /// <summary>
    /// Provides statistical counting and management services.
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        #region Fields

        private readonly object _statisticsLock = new object();
        protected ILogger _log;
        protected IOptionsRepository _optionsRepository;
        private DispatcherTimer _refreshTime;
        private DispatcherTimer _savingTime;
        private ResultStatistics _resultStatistics = new ResultStatistics();

#if STATISTICS_EMULATION
        private DispatcherTimer _liveTime;
        private Random _rnd = new Random();
#endif

        #endregion

        #region Properties (IStatisticsService)

        /// <summary>
        /// Gets the default instance of the <see cref="StatisticsService"/>.
        /// </summary>
        public static IStatisticsService Default { get; private set; }

        /// <summary>
        /// Gets the instance of the service.
        /// </summary>
        public object Instance => Default;

        /// <summary>
        /// Gets the options used to configure the statistical service.
        /// </summary>
        public virtual ResultStatisticsOptions Options { get; protected set; } = new ResultStatisticsOptions();

        /// <summary>
        /// Gets the description of the service.
        /// </summary>
        public string Description { get; protected set; } = "Statistics counting service";

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string Name { get; protected set; } = nameof(StatisticsService);

        /// <summary>
        /// Gets the current status of the service.
        /// </summary>
        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        /// <summary>
        /// Gets the statistical result information.
        /// </summary>
        public IResultStatisticsInfo Get => _resultStatistics;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsService"/> class.
        /// </summary>
        /// <param name="log">An optional logger instance.</param>
        /// <param name="optionsRepository">An optional repository for options.</param>
        public StatisticsService(ILogger log = null, IOptionsRepository optionsRepository = null)
        {
            _log = log;
            _optionsRepository = optionsRepository;
            if (Default == null)
                Default = this;
        }

        #endregion

        #region Public Methods (IStatisticsService)

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Persist()
        {
            lock (_statisticsLock)
            {
                try
                {
                    _optionsRepository?.Update(nameof(ResultStatisticsOptions), (a) => a = Options);
                }
                catch (Exception ex)
                {
                    _log?.Fatal($"Exception during saving state of {Description}", ex);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Refresh(DateTime dateTime = default)
        {
            if (dateTime == default)
                dateTime = DateTime.Now;

            lock (_statisticsLock)
            {
                _resultStatistics.Refresh(dateTime);
                RaiseStatisticsChanged(this, new StatisticsEventArgs(_resultStatistics));
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Reset()
        {
            lock (_statisticsLock)
            {
                _resultStatistics.Reset();
                RaiseStatisticsChanged(this, new StatisticsEventArgs(_resultStatistics));
                Persist();
                ResetRefreshTimer();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Setup()
        {
            try
            {
                var options = _optionsRepository.TryGet(nameof(ResultStatisticsOptions), ResultStatisticsOptions.DefaultInstance);
                Options = options;
                _resultStatistics.Configure(options);

                _savingTime = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };
                _refreshTime = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) };

#if STATISTICS_EMULATION
                _liveTime = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
#endif
            }
            catch (Exception ex)
            {
                _log?.Error($"Exception during configuration of {Description}", ex);
                throw new InvalidConfigurationParameterException(
                    $"Exception during configuration of {Description} of type {nameof(StatisticsService)}", ex);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Start()
        {
            if (Status == ServiceStatus.Stopped)
            {
                Status = ServiceStatus.Started;
                try
                {
                    _log?.Info($"Starting {Description}");

                    _savingTime.Tick += _savingTimerTick;
                    _savingTime.Start();

                    _refreshTime.Tick += _refreshTimerTick;
                    _refreshTime.Start();

                    Refresh();

#if STATISTICS_EMULATION
                    _liveTime.Tick += _emulationTimerTick;
                    _liveTime.Start();
#endif
                    _log?.Info($"Started {Description}");
                }
                catch (Exception ex)
                {
                    _log?.Fatal($"Exception during starting {Description}", ex);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                try
                {
                    _log?.Info($"Stopping {Description}");

                    _savingTime.Tick -= _savingTimerTick;
                    _savingTime.Stop();

                    _refreshTime.Tick -= _refreshTimerTick;
                    _refreshTime.Stop();

#if STATISTICS_EMULATION
                    _liveTime.Tick -= _emulationTimerTick;
#endif

                    Persist();
                    _log?.Info($"Stopped {Description}");
                }
                catch (Exception ex)
                {
                    _log?.Error($"Exception during stopping {Description}", ex);
                }
                Status = ServiceStatus.Stopped;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Update(ProductResult productResult, DateTime dateTime = default)
        {
            lock (_statisticsLock)
            {
                try
                {
                    bool shouldSave = false;
                    if (!_resultStatistics.IsValid(productResult))
                    {
                        _resultStatistics.Repair(productResult);
                        shouldSave = true;
                    }

                    _resultStatistics.Update(productResult, dateTime);
                    RaiseStatisticsChanged(this, new StatisticsEventArgs(_resultStatistics));

                    if (shouldSave)
                        Persist();

                    ResetRefreshTimer();
                }
                catch (Exception ex)
                {
                    _log?.Fatal($"Exception during saving state of {Description}", ex);
                }
            }
        }

        #endregion

        #region Private Methods

        private void _refreshTimerTick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void _savingTimerTick(object sender, EventArgs e)
        {
            lock (_statisticsLock)
            {
                Persist();
            }
        }

        private void RaiseStatisticsChanged(object sender, StatisticsEventArgs e)
        {
            try
            {
                StatisticsChanged?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogToConsole();
            }
        }

        private void ResetRefreshTimer()
        {
            _refreshTime.Stop();
            _refreshTime.Start();
        }

#if STATISTICS_EMULATION
        private void _emulationTimerTick(object sender, EventArgs e)
        {
            var foreignObjectsResult = _rnd.Next(99) >= 5;
            var meatClassifierResult = _rnd.Next(99) >= 5;

            var productResult = new ProductResult
            {
                Info = new ProductInfoResult
                {
                    DateTime = DateTime.Now,
                    Enabled = true,
                    Success = true,
                    Result = foreignObjectsResult && meatClassifierResult,
                },
                Inspections = new List<InspectionResult>
                {
                    new InspectionResult
                    {
                        Name = "ForeignObjects",
                        Enabled = true,
                        Success = true,
                        PrevResult = true,
                        Result = foreignObjectsResult,
                        Measurables = new List<ValueResult>
                        {
                            new ValueResult
                            {
                                Name = "Score",
                                Type = Entities.ValueType.Float,
                                Value = _rnd.NextDouble()
                            }
                        }
                    },
                    new InspectionResult
                    {
                        Name = "MeatClassifier",
                        Enabled = true,
                        Success = true,
                        PrevResult = true,
                        Result = meatClassifierResult,
                        Measurables = new List<ValueResult>
                        {
                            new ValueResult
                            {
                                Name = "Score",
                                Type = Entities.ValueType.Float,
                                Value = _rnd.NextDouble()
                            }
                        }
                    }
                }
            };

            lock (_statisticsLock)
            {
                _resultStatistics.Update(productResult);
                RaiseStatisticsChanged(this, new StatisticsEventArgs(_resultStatistics, productResult));
            }
        }
#endif

        #endregion

        #region Events (IStatisticsService)

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<StatisticsEventArgs> StatisticsChanged;

        #endregion
    }
}

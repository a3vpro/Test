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
using VisionNet.Core.Abstractions;
using VisionNet.Core.Exceptions;
using VisionNet.Vision.Core;
using Castle.Core.Logging;
using VisionNet.Application;
using System.Windows.Threading;
using VisionNet.DataBases;
using Conditions;
using VisionNet.Core.Security;
using System.Collections.Generic;

namespace VisionNet.Vision.Services
{
    public class AdvancedStatisticsService : IStatisticsService
    {
        private SQLiteApi _sqlApi;
        private string _connectionString;

        private DispatcherTimer _deleteOldRecordsTime;

        protected ILogger _log;
        protected IOptionsRepository _optionsRepository;
        private ResultStatistics _resultStatistics = new ResultStatistics();

        public static IStatisticsService Default { get; private set; }
        public object Instance => Default;

        public string Name { get; protected set; } = nameof(StatisticsService);

        public string Description { get; protected set; } = "Statistics counting service";

        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        public virtual ResultStatisticsOptions Options { get; protected set; } = new ResultStatisticsOptions();

        public AdvancedStatisticsService(ILogger log = null, IOptionsRepository optionsRepository = null)
        {
            _log = log;
            _optionsRepository = optionsRepository;

            if (Default == null)
                Default = this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Setup()
        {
            try
            {
                // Start Services
                var options = _optionsRepository.TryGet(nameof(ResultStatisticsOptions), ResultStatisticsOptions.Default);

                _sqlApi = new SQLiteApi();

                if (options.DBConnection.UseConnectionString)
                {
                    options.DBConnection.ConnectionString.Requires(nameof(options.DBConnection.ConnectionString))
                        .IsNotNullOrWhiteSpace()
                        .Evaluate(_sqlApi.ValidateConnectionString(options.DBConnection.ConnectionString));
                }
                else
                {
                    options.DBConnection.ServerName.Requires(nameof(options.DBConnection.ServerName)).IsNotNullOrWhiteSpace();
                    options.DBConnection.Database.Requires(nameof(options.DBConnection.Database)).IsNotNullOrWhiteSpace();
                    options.DBConnection.User.Requires(nameof(options.DBConnection.User)).IsNotNullOrWhiteSpace();
                    options.DBConnection.EncodedPassword.Requires(nameof(options.DBConnection.EncodedPassword)).IsLongerOrEqual(2);
                }

                // Configuramos la conexión a la base de datos                
                _connectionString = options.DBConnection.ConnectionString;
                if (!options.DBConnection.UseConnectionString)
                    _connectionString = _sqlApi.CreateConnectionString(options.DBConnection.ServerName, options.DBConnection.Database, options.DBConnection.User, options.DBConnection.EncodedPassword.Decript().ToString());

                _deleteOldRecordsTime = new DispatcherTimer();
                _deleteOldRecordsTime.Interval = TimeSpan.FromSeconds(60);

                Options = options;
            }
            catch (Exception ex)
            {
                _log?.Error($"Exception during configuration of {Description}", ex);
                throw new InvalidConfigurationParameterException($"Exception during configuration of {Description} of type {nameof(StatisticsService)}", ex);
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

                    // Delete old records
                    _deleteOldRecordsTime.Tick += _deleteOldRecordsTimerTick;
                    _deleteOldRecordsTime.Start();

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

                    _deleteOldRecordsTime.Tick -= _deleteOldRecordsTimerTick;
                    _deleteOldRecordsTime.Stop();

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
        public IResultStatisticsInfo Get => _resultStatistics;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Reset()
        {
            // Código aquí
            RaiseStatisticsChanged(this, new StatisticsEventArgs(_resultStatistics));
            Persist();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Persist()
        {
            try
            {
                // Código aquí
            }
            catch (Exception ex)
            {
                _log?.Fatal($"Exception during deleteOldRecords state of {Description}", ex);
            }
        }

        /// <summary>
        /// Update the statistics values
        /// </summary>
        /// <param name="inspectionResult"></param>
        /// <param name="dateTime"></param>
        public void Update(ProductResult productResult, DateTime dateTime = default)
        {
            try
            {
                using (var connection = _sqlApi.Connect(_connectionString))
                {
                    //connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Mapea el producto a un diccionario con los valores que se insertarán
                            var statisticsRecords = new StatisticsMapper().MapToStatistics(productResult);

                            // Inserta los registros
                            foreach (var statisticsRecord in statisticsRecords)
                            {
                                var statisticsParams = new Dictionary<string, object>
                                {
                                    //{"@Id", statisticsRecord.Id}, // Si Id es autogenerado, podría omitirse
                                    {"@ProductId", statisticsRecord.ProductId},
                                    {"@DateTime", statisticsRecord.DateTime},
                                    {"@InspectionName", statisticsRecord.InspectionName},
                                    {"@MatchingCriteria", statisticsRecord.MatchingCriteria}
                                };

                                _sqlApi.Insert(connection, transaction, "Inspections", statisticsParams);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                    //connection.Close();
                }
            }
            catch (Exception ex)
            {
                _log?.Fatal($"Exception during deleteOldRecords state of {Description}", ex);
            }
        }

        private void _deleteOldRecordsTimerTick(object sender, EventArgs e)
        {
            //
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
        public event EventHandler<StatisticsEventArgs> StatisticsChanged;
    }
}
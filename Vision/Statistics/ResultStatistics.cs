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
using System.Linq;
using VisionNet.Core.Exceptions;
using Microsoft.Extensions.Options;
using VisionNet.Vision.AI;

namespace VisionNet.Vision.Core
{
    public class ResultStatistics : IResultStatistics, IConfigureOptions<ResultStatisticsOptions>
    {
        private object _lockObject = new object();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ResultStatisticsOptions Options { get; private set; } = new ResultStatisticsOptions();

        private InspectionStatistics _global = new InspectionStatistics();
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IInspectionStatisticsInfo Global => _global;

        private List<InspectionStatistics> _inspections = new List<InspectionStatistics>();
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IReadOnlyList<IInspectionStatisticsInfo> GetAllInspections() => _inspections;

        public IStatisticInfo Get(TimeSpan maxDuration)
        {
            return _global.Get(maxDuration);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IInspectionStatisticsInfo this[string index]
        {
            get
            {
                lock (_lockObject)
                {
                    if (!_isValid(index))
                        _repair(index);
                }

                return _inspections.FirstOrDefault(i => i.Name == index);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public long Population => _global.Population;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public long GetPopulation(TimeSpan maxDuration)
        {
            return _global.GetPopulation(maxDuration);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public long MatchingCriteria => _global.MatchingCriteria;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public long GetMatchingCriteria(TimeSpan maxDuration)
        {
            return _global.GetMatchingCriteria(maxDuration);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public double? Rate => _global.Rate;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public double? GetRate(TimeSpan maxDuration)
        {
            return _global.GetRate(maxDuration);
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Configure(ResultStatisticsOptions options)
        {
            try
            {
                _global.Configure(options.Global);

                _inspections.Clear();
                foreach (var opt in options.Inspections)
                {
                    var insp = new InspectionStatistics();
                    insp.Configure(opt);
                    _inspections.Add(insp);
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
                foreach (var inspection in _inspections)
                    inspection.Refresh(dateTime);
            }
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
                foreach (var inspection in _inspections)
                    inspection.Reset(dateTime);
            }
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
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Update(ProductResult productResult, DateTime dateTime = default)
        {
            lock (_lockObject)
            {
                if (dateTime == default)
                    dateTime = DateTime.Now;

                _global.Update(productResult.Info.Result, dateTime);
                if (!productResult.Info.Result)
                {
                    // Obtenemos todas las inspecciones (estándar + segmentación) ya desglosadas por Populate()
                    var allPopulatedInspections = productResult.Inspections
                        .SelectMany(i => i.Populate());

                    // Filtramos por IncludeInStats y agrupamos por nombre para obtener el resultado
                    var perNameResults = allPopulatedInspections
                        .Where(i => i.IncludeInStats)
                        .GroupBy(i => i.Name, (k, g) => new { Name = k, Result = g.All(x => x.Result) });

                    // 1) Resultado por “NombreBase”  ─────────────────────────────────────────────
                    //    - Me quedo sólo con los nombres que llevan punto.
                    //    - Obtengo la parte anterior al primer punto.
                    //    - Agrupo nuevamente por esa parte.
                    var baseNameResults = perNameResults
                        .Where(r => r.Name.Contains('.'))
                        .Select(r => new
                        {
                            BaseName = r.Name.Substring(0, r.Name.IndexOf('.')),
                            r.Result
                        })
                        .GroupBy(r => r.BaseName)
                        .Select(g => new
                        {
                            Name = g.Key,                   // aquí el nombre es el NombreBase
                            Result = g.All(x => x.Result)     // true si TODOS los hijos son true
                        });

                    // 2) Mezclo ambos conjuntos  ─────────────────────────────────────────────────
                    //    Si prefieres que el NombreBase machaque al existente (o viceversa)
                    //    puedes cambiar DistinctBy, OrderBy, etc.
                    var inspectionResults = perNameResults
                        .Concat(baseNameResults)
                        .GroupBy(r => r.Name)                 // quita duplicados si los hubiera
                        .Select(g => g.First())               // elige la primera ocurrencia
                        .ToList();

                    // Actualizamos nuestras inspecciones locales
                    foreach (var inspection in _inspections)
                    {
                        // Busca un resultado por el nombre
                        var resultForThisInspection = inspectionResults.FirstOrDefault(r => r.Name == inspection.Name);

                        // Si no se encuentra, damos por correcto (true), si se encuentra, tomamos su valor.
                        bool result = resultForThisInspection?.Result ?? true;

                        inspection.Update(result, dateTime);
                    }
                }
            }
        }

        public bool IsValid(ProductResult productResult)
        {
            bool result = true;

            lock (_lockObject)
            {
                // Obtenemos todas las inspecciones (estándar y segmentación) tras llamarlas a Populate()
                var allPopulatedInspections = productResult.Inspections
                    .SelectMany(i => i.Populate())
                    .Where(i => i.IncludeInStats);

                // Por cada inspección resultante, comprobamos si existe entre _inspections
                foreach (var insp in allPopulatedInspections)
                    result &= _isValid(insp.Name);
            }

            return result;
        }

        public void Repair(ProductResult productResult)
        {
            lock (_lockObject)
            {
                // Obtenemos todas las inspecciones (estándar y segmentación) tras llamar a Populate()
                var allPopulatedInspections = productResult.Inspections
                    .SelectMany(i => i.Populate())
                    .Where(i => i.IncludeInStats);

                // Por cada inspección resultante, si no existe en _inspections, la creamos y añadimos.
                foreach (var inspResult in allPopulatedInspections)
                    _repair(inspResult.Name);
            }
        }

        private bool _isValid(string inspectionName)
        {
            if (!_inspections.Any(i => i.Name == inspectionName))
                return false;

            return true;
        }

        private void _repair(string inspectionName)
        {

            if (!_inspections.Any(i => i.Name == inspectionName))
            {
                var insp = _createInspection(inspectionName);
                Options.Inspections.Add(insp.Options);
                _inspections.Add(insp);
            }
        }

        private InspectionStatistics _createInspection(string inspectionName)
        {
            var opt = new InspectionStatisticsOptions
            {
                Name = inspectionName,
                Global = new SingleStatisticsOptions(),
                Timed = Options.Global.Timed.Select(t => new TimedStatisticsOptions
                {
                    TimeInterval = t.TimeInterval,
                    MaxDuration = t.MaxDuration,
                }).ToList(),
            };

            var insp = new InspectionStatistics();
            insp.Configure(opt);
            insp.Reset();
            return insp;
        }
    }
}
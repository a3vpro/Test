//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using VisionNet.Core.Abstractions;
using Castle.Core.Logging;
using VisionNet.Application;
using System.Linq;
using Conditions;
using System;
using VisionNet.Core.Exceptions;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;
using VisionNet.Core.Events;

namespace VisionNet.Vision.Services
{
    public class VisionService : IVisionService
    {
        private readonly ILogger _log;
        private readonly IOptionsRepository _optionsRepository;
        private readonly IServiceRepository _serviceRepository;

        private VisionLoggedFactory _VisionFactory;
        private ConcurrentDictionary<string, IVisionConfigurable> _Vision = new ConcurrentDictionary<string, IVisionConfigurable>();

        public static IVisionService Default { get; private set; }
        public object Instance => Default;

        public string Name { get; protected set; } = nameof(VisionService);

        public string Description { get; protected set; } = "Vision service";

        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        public virtual VisionOptions Options { get; protected set; } = new VisionOptions();

        public VisionService(ILogger log = null, IOptionsRepository optionsRepository = null, IServiceRepository serviceRepository = null)
        {
            _log = log;
            _optionsRepository = optionsRepository;
            _serviceRepository = serviceRepository;

            if (Default == null)
                Default = this;

            _VisionFactory = new VisionLoggedFactory(_serviceRepository, _log);
        }

        public void Setup()
        {
            try
            {
                // Start Services
                var options = _optionsRepository.TryGet(nameof(VisionOptions), VisionOptions.Default);

                options.Vision.Select(c => string.IsNullOrWhiteSpace(c.Index))
                    .Requires(nameof(options.Vision))
                    .DoesNotContain(true);
                options.Vision.GroupBy(x => x.Index).Where(g => g.Count() > 1)
                    .Requires(nameof(options.Vision))
                    .IsEmpty();
                options.Vision.Select(c => string.IsNullOrWhiteSpace(c.Name))
                    .Requires(nameof(options.Vision))
                    .DoesNotContain(true);

                foreach (var VisionOption in options.Vision)
                {
                    _VisionFactory.VisionName = VisionOption.Name;
                    var Vision = _VisionFactory.Create();
                    Vision.Configure(VisionOption);
                    _Vision[VisionOption.Index] = Vision;
                }

                Options = options;
            }
            catch (Exception ex)
            {
                var newEx = new InvalidConfigurationParameterException($"Exception during configuration of {nameof(VisionService)}", ex);
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }
        }

        public void Start()
        {
            if (Status == ServiceStatus.Stopped)
            {
                try
                {
                    _log?.Info($"Startting {Description}");

                    foreach (var Vision in GetAll())
                        Vision.Start();

                    foreach (var Vision in _Vision.Values)
                        Vision.StateChanged += RaiseAnyStateChanged;

                    _log?.Info($"Started {Description}");
                }
                catch (Exception ex)
                {
                    _log?.Fatal($"Exception during starting {Description}", ex);
                }
                Status = ServiceStatus.Started;
            }
        }

        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                try
                {
                    _log?.Info($"Stopping {Description}");

                    foreach (var Vision in _Vision.Values)
                        Vision.StateChanged -= RaiseAnyStateChanged;

                    foreach (var Vision in GetAll())
                        Vision.Stop();

                    _log?.Info($"Stopped {Description}");
                }
                catch (Exception ex)
                {
                    _log?.Error($"Exception during stopping {Description}", ex);
                }

                Status = ServiceStatus.Stopped;
            }
        }

        public int Count()
        {
            return _Vision.Count();
        }

        public bool Exists(string id)
        {
            return _Vision.ContainsKey(id);
        }

        public IVisionConfigurable Get(string id)
        {
            if (!Exists(id))
            {
                var newEx = new ArgumentException($"{id} not found in existing Vision. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            return _Vision[id];
        }

        public T Get<T>(string id) where T : IVisionConfigurable, new()
        {
            if (!Exists(id))
            {
                var newEx = new ArgumentException($"{id} not found in existing Vision. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            return (T)Get(id);
        }

        public IList<IVisionConfigurable> GetAll()
        {
            return _Vision.Values.ToList();
        }

        private void RaiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                _log?.Error($"Error in {Description}", eventArgs.GetException());
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        private void RaiseAnyStateChanged(object sender, StateChangedEventArgs<string> e)
        {
            try
            {
                if (sender is IVisionConfigurable)
                {
                    var name = ((IVisionConfigurable)sender).Name;
                    AnyStateChanged?.Invoke(sender, e);
                }
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<StateChangedEventArgs<string>> AnyStateChanged;
    }
}

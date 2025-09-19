using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Exceptions;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public class VisionRepository : IVisionRepository
    {
        private VisionLoggedFactory _factory;

        private ConcurrentDictionary<string, IVisionFunction> _Vision = new ConcurrentDictionary<string, IVisionFunction>();

        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;
        public virtual VisionOptions Options { get; protected set; } = new VisionOptions();

        public VisionRepository(VisionLoggedFactory factory)
        {
            _factory = factory;
        }

        public void Configure(VisionOptions options)
        {
            try
            {
                Options = options;

                foreach (var VisionOption in options.VisionFunctions)
                {
                    _factory.VisionFunctionType = VisionOption.VisionFunctionType;
                    _factory.VisionFunctionName = VisionOption.Index;
                    var Vision = _factory.Create();
                    Vision.Configure(VisionOption);
                    _Vision[VisionOption.Name] = Vision;
                }
            }
            catch (Exception ex)
            {
                var newEx = new InvalidConfigurationParameterException($"Exception during configuration of Vision Repository", ex);
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
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

        public IVisionFunction Get(string id)
        {
            if (!Exists(id))
            {
                var newEx = new ArgumentException($"{id} not found in existing Vision. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            return _Vision[id];
        }

        public T Get<T>(string id) where T : IVisionFunction, new()
        {
            if (!Exists(id))
            {
                var newEx = new ArgumentException($"{id} not found in existing Vision. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            return (T)Get(id);
        }

        public IList<IVisionFunction> GetAll()
        {
            return _Vision.Values.ToList();
        }

        private void RaiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch { }
        }
        public event EventHandler<ErrorEventArgs> ExceptionRaised;
    }
}

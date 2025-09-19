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
using Castle.Core.Logging;
using VisionNet.Application;
using VisionNet.Vision.Core;
using VisionNet.Vision.Logging;

namespace VisionNet.Vision.Services
{
    public class VisionLoggedFactory : VisionFactory
    {
        private ILogger _log;

        public VisionLoggedFactory(IServiceRepository serviceRepository, ILogger log): base(serviceRepository)
        {
            _log = log;
        }

        /// <summary>
        /// Constructor of the instance
        /// </summary>
        /// <returns>Instance of the class</returns>        
        protected override IVisionFunction NewInstance()
        {
            var vision = base.NewInstance();

            // Log
            if (_log != null)
                new VisionFunctionLogger(vision, _log);

            return vision;
        }

        public static IVisionFunction CreateNew(VisionFunctionType visionFunctionType, string visionName, IServiceRepository serviceRepository, ILogger log)
        {
            var factory = new VisionLoggedFactory(serviceRepository, log)
            {
                VisionFunctionType = visionFunctionType,
                VisionFunctionName = visionName,
            };
            return factory.Create();
        }
    }
}

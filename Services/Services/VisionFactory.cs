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
using VisionNet.Application;
using VisionNet.Core.Patterns;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public class VisionFactory : Factory<IVisionFunction>
    {
        private IServiceRepository _serviceRepository;

        /// <summary>
        /// Name of the vision function
        /// </summary>
        public VisionFunctionType VisionFunctionType { get; set; } = VisionFunctionType.Custom;

        /// <summary>
        /// Name of the vision function
        /// </summary>
        public string VisionFunctionName { get; set; } = "";

        public VisionFactory(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        /// <summary>
        /// Constructor of the instance
        /// </summary>
        /// <returns>Instance of the class</returns>        
        protected override IVisionFunction NewInstance()
        {
            var visionKey = VisionClassKeyResolver.GetKeyClass(VisionFunctionType, VisionFunctionName);
            var vision = _serviceRepository.Get<IVisionFunction>(visionKey);

            return vision;
        }

        public static IVisionFunction CreateNew(VisionFunctionType visionFunctionType, string VisionName, IServiceRepository serviceRepository)
        {
            var factory = new VisionFactory(serviceRepository)
            {
                VisionFunctionType = visionFunctionType,
                VisionFunctionName = VisionName,
            };
            return factory.Create();
        }
    }
}

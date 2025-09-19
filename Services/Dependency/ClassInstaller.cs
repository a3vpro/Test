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
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Core;

namespace VisionNet.Vision.Services
{
    public class ClassInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (!container.Kernel.HasComponent(typeof(IVisionRepository)) &&
                !container.Kernel.HasComponent(typeof(IVisionService)) &&
                !container.Kernel.HasComponent(typeof(IVisionResultService)))
            {
                container.Register(Component.For<IVisionRepository, IVisionService, IVisionResultService>()
                .ImplementedBy<VisionService>()
                .LifeStyle.Is(LifestyleType.Singleton));
            }

            container.Register(Component.For<IStatisticsService, IStatisticsProvider>()
                           .ImplementedBy<StatisticsService>()
                           .LifeStyle.Is(LifestyleType.Singleton));
        }
    }
}

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
using VisionNet.Vision.Core;
using VisionNet.Application.Requisites;
using VisionNet.Core.Requisites;

namespace VisionNet.Vision.VisionPro
{
    public class ClassInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IVisionFunction>()
               .ImplementedBy<VisionProVisionFunction>()
               .LifeStyle.Is(LifestyleType.Transient)
               .Named(VisionClassKeyResolver.GetKeyClass(VisionFunctionType.VisionProToolBlock)));

            container.Register(Component.For<IApplicationRequisite>()
               .ImplementedBy<VisionProLicensePresent>()
               .LifeStyle.Is(LifestyleType.Transient)
               .Named(ApplicationRequisiteClassKeyResolver.GetKeyClass(RequisiteType.License, "VisionPro")));
        }
    }
}

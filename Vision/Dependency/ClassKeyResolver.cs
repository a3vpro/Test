//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 07-12-2023
//
// Last Modified By : aibanez
// Last Modified On : 07-12-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using VisionNet.Core.IOC;

namespace VisionNet.Vision.Core
{
    public class VisionClassKeyResolver: ClassKeyResolver<VisionFunctionType>
    {
        protected override string ClassType => "VisionFunction";

        public override string Resolve(VisionFunctionType type, string className = "")
        {
            var visionFunctionKey = type == VisionFunctionType.Custom
                ? className
                : type.ToString();

            return $"{visionFunctionKey}.{ClassType}";
        }

        public static string GetKeyClass(VisionFunctionType type, string className = "")
        {
            return new VisionClassKeyResolver().Resolve(type, className);
        }
    }
}
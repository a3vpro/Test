//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 01-06-2020
//
// Last Modified By : aibanez
// Last Modified On : 29-09-2020
// Description      : v1.4.2
//
// Copyright        : (C)  2020 by Sothis. All rights reserved.       
//----------------------------------------------------------------------------

using Conditions;
using VisionNet.Vision.Core;
using System.Collections.Generic;
using VisionNet.Image;
using System.Linq;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using VisionNet.Conditions.Paths;
using VisionNet.IO.Paths;
using VisionNet.Core.Types;
using System;
using VisionNet.Image.VisionPro;
using System.Collections;
using VisionNet.Core.Enums;

namespace VisionNet.Vision.VisionPro
{
    public class VisionProVisionFunction
        : VisionProVisionFunctionBase<
            VisionFunctionOptions,
            InputParametersCollection,
            OutputParametersCollection>
    {
        public VisionProVisionFunction() : base()
        {
        }

        public override IInputParametersCollection GetInputParameters(VisionMessage vm)
        {
            // Llamamos a la función _enhancerFunction.TrySetInputParameters
            return new InputParametersCollection();
        }
    }
}
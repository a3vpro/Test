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
using System.Collections.Generic;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public class InputParametersCollection : ParametersCollection, IInputParametersCollection
    {
        public override ParameterDirection Direction => ParameterDirection.Input;

        /// <inheritdoc/>
        [Parameter(nameof(Enabled), 
            nameof(Enabled), 
            nameof(Enabled), 
            nameof(Enabled), 
            ParameterDirection.Input, 
            ParameterSource.Constant, 
            true, 
            ParameterScope.Initialization, 
            BasicTypeCode.Boolean, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public bool Enabled
        {
            get => this.ToBool(nameof(Enabled));
            set => TrySetValue(nameof(Enabled), value);
        }

        public IParameter EnabledParameter =>
            Get(nameof(Enabled));

        /// <inheritdoc/>
        [Parameter(nameof(PrevResult), 
            nameof(PrevResult), 
            nameof(PrevResult),
            "Previous result", 
            ParameterDirection.Input, 
            ParameterSource.Runtime, 
            true, 
            ParameterScope.Execution,
            BasicTypeCode.Boolean, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public bool PrevResult
        {
            get => this.ToBool(nameof(PrevResult));
            //set => TrySetValue(nameof(PrevResult), value);
        }

        public bool TrySetPrevResult(bool prevResult)
        {
            return TrySetValue(nameof(PrevResult), prevResult);
        }

        public IParameter PrevResultParameter =>
            Get(nameof(PrevResult));
    }
}

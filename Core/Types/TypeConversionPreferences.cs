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
using System;

namespace VisionNet.Core.Types
{
    [Flags]
    public enum TypeConversionPreferences
    {
        None = 0,
        StringToBooleanLanguageInvariant = 1,
        StringToBooleanAllowsNumbers = 1 << 1,
        NumberClamp = 1 << 2,
        UseRegexIfNecessary = 1 << 3,
        StringToBooleanPermissive = StringToBooleanLanguageInvariant | StringToBooleanAllowsNumbers,
        StringToNumberPermissive = NumberClamp | UseRegexIfNecessary,
    }
}

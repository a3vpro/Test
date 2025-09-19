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
namespace VisionNet.Core.Types
{
    public enum BasicTypeCode
    {
        IntegerNumber = 1,
        FloatingPointNumber = 2,
        String = 3,
        Boolean = 4,
        DateTime = 5,
        Object= 6,
        Image= 7,
        Graphic = 8,
        NotSupported = 0 // Para los TypeCode que no caen en una categoría específica
    }
}

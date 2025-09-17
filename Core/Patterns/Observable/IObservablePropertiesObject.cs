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

using System.ComponentModel;

namespace VisionNet.Core.Patterns
{
    public interface IObservablePropertiesObject
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}

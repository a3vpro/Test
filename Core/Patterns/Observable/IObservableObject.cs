//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 28-08-2021
// Description      : v1.0.0
//
// Copyright        : (C)  2021 by Sothis. All rights reserved.       
//----------------------------------------------------------------------------

using System.ComponentModel;

namespace VisionNet.Core.Patterns
{
    public interface IObservableObject
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}

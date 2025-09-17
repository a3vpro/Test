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
namespace VisionNet.Core.Events
{
    /// <summary>
    /// Represents a method that will handle an event with a custom event arguments of type <typeparamref name="TEventArgs"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event data passed to the event handler.</typeparam>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void CustomEventHandler<in TEventArgs>(object sender, TEventArgs e);

}

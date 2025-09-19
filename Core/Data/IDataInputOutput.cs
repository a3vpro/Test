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
namespace VisionNet.Core.Data
{
    /// <summary>
    /// Defines an interface for handling both input and output data. 
    /// This interface extends <see cref="IDataOutput{O}"/> for output and <see cref="IDataInput{I}"/> for input.
    /// </summary>
    /// <typeparam name="I">The type of input data.</typeparam>
    /// <typeparam name="O">The type of output data.</typeparam>
    public interface IDataInputOutput<I, O> : IDataOutput<O>, IDataInput<I>
    {
    }

}

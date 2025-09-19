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
using Microsoft.Extensions.Options;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Data
{
    /// <summary>
    /// Defines an interface for filtering data with input and output of type <typeparamref name="TInOut"/> and options of type <typeparamref name="TOptions"/>.
    /// This interface extends the functionality of <see cref="IDataInputOutput{TInOut, TInOut}"/>, <see cref="IExecutable"/>, <see cref="IExecutionObservable{TInOut}"/>, and <see cref="IConfigureOptions{TOptions}"/>.
    /// </summary>
    /// <typeparam name="TInOut">The type of input and output data to be filtered.</typeparam>
    /// <typeparam name="TOptions">The type of options used for configuration, constrained to a class type.</typeparam>
    public interface IDataFilterer<TInOut, TOptions> : IDataInputOutput<TInOut, TInOut>, IExecutable, IExecutionObservable<TInOut>, IConfigureOptions<TOptions>
        where TOptions : class
    {
    }

}

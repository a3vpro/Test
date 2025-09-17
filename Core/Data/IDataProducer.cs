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
    /// Defines an interface for producing data of type <typeparamref name="TOut"/> with configuration options of type <typeparamref name="TOptions"/>.
    /// This interface extends <see cref="IDataOutput{TOut}"/>, <see cref="IExecutable"/>, <see cref="IExecutionObservable{TOut}"/>, and <see cref="IConfigureOptions{TOptions}"/>.
    /// </summary>
    /// <typeparam name="TOut">The type of data to be produced.</typeparam>
    /// <typeparam name="TOptions">The type of options used for configuration, constrained to a class type.</typeparam>
    public interface IDataProducer<TOut, TOptions> : IDataOutput<TOut>, IExecutable, IExecutionObservable<TOut>, IConfigureOptions<TOptions>
        where TOptions : class
    {
    }

}

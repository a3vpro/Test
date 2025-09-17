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
    /// Defines an interface for consuming data with input of type <typeparamref name="TIn"/> and options of type <typeparamref name="TOptions"/>.
    /// This interface extends the functionality of <see cref="IDataInput{TIn}"/>, <see cref="IExecutable"/>, <see cref="IExecutionObservable{TIn}"/>, and <see cref="IConfigureOptions{TOptions}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of input data to be consumed.</typeparam>
    /// <typeparam name="TOptions">The type of options used for configuration, constrained to a class type.</typeparam>
    public interface IDataConsumer<TIn, TOptions> : IDataInput<TIn>, IExecutable, IExecutionObservable<TIn>, IConfigureOptions<TOptions>
        where TOptions : class
    {
    }
}

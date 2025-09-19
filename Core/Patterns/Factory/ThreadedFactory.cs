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
using System.Collections.Concurrent;
using System.Threading;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// A factory that creates and manages instances of a specified type <typeparamref name="T"/> 
    /// specific to each thread. If an instance already exists for the current thread, the existing instance is returned.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates and manages for each thread.</typeparam>
    public class ThreadedFactory<T> : FactoryAppender<T>
    {
        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> if it does not already exist for the current thread.
        /// If an instance of <typeparamref name="T"/> already exists for the current thread, that instance is returned.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="T"/> associated with the current thread.</returns>
        public override T Create()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            if (!Instances.TryGetValue(threadId, out T result))
            {
                if (result == null)
                    result = NewInstance();
                Instances[threadId] = result;
            }

            return result;
        }

        /// <summary>
        /// A dictionary that holds instances of type <typeparamref name="T"/> indexed by the thread ID.
        /// Each thread has its own unique instance of <typeparamref name="T"/>.
        /// </summary>
        protected static ConcurrentDictionary<int, T> Instances { get; set; } = new ConcurrentDictionary<int, T>();

        /// <summary>
        /// A wrapper around the <see cref="Create"/> method that allows the creation of an object of type <typeparamref name="T"/>
        /// and links it to another factory. This is useful for creating new objects with default values while also allowing
        /// for custom values from the provided factory.
        /// </summary>
        /// <param name="factory">The factory to use for creating the object.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> specific to the current thread.</returns>
        public new static T CreateNew(IFactory<T> factory)
        {
            var factoryAppender = new ThreadedFactory<T>();
            factoryAppender.LinkTo(factory);
            return factoryAppender.Create();
        }
    }
}
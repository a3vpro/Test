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
namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// A factory that creates and manages a single instance of a specified type <typeparamref name="T"/>.
    /// If the instance already exists, the existing instance is returned.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates and manages as a singleton.</typeparam>
    public class SingletonFactory<T> : FactoryAppender<T>
    {
        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> if it does not already exist.
        /// If an instance of <typeparamref name="T"/> already exists, that instance is returned.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="T"/>.</returns>
        public override T Create()
        {
            if (Instance == null)
                Instance = NewInstance();
            return Instance;
        }

        /// <summary>
        /// The singleton instance of type <typeparamref name="T"/>. This is a shared instance used across calls.
        /// </summary>
        protected static T Instance { get; set; }

        /// <summary>
        /// A static method that creates an instance of the <see cref="SingletonFactory{T}"/> class,
        /// links it to the provided factory, and returns the singleton instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="factory">The factory that will be used to create the object.</param>
        /// <returns>The object created by the factory, which is managed as a singleton.</returns>
        public new static T CreateNew(IFactory<T> factory)
        {
            var factoryAppender = new SingletonFactory<T>();
            factoryAppender.LinkTo(factory);
            return factoryAppender.Create();
        }
    }
}

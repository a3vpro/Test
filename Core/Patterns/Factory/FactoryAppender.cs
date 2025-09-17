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
    /// A specialized factory class that extends <see cref="Factory{T}"/> and adds the ability to link to another factory.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates.</typeparam>
    public class FactoryAppender<T> : Factory<T>, IFactoryAppender<T>
    {
        /// <summary>
        /// Gets or sets the linked factory that can be used to create instances of <typeparamref name="T"/>.
        /// </summary>
        protected IFactory<T> Factory { get; set; }

        /// <summary>
        /// Links the current factory to another factory, allowing for a chain of factories.
        /// This enables the creation of objects in sequence, passing objects between factories.
        /// </summary>
        /// <param name="factory">The factory to link to.</param>
        /// <returns>The factory that was passed in.</returns>
        public virtual void LinkTo(IFactory<T> factory)
        {
            Factory = factory;
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/> by delegating the creation process to the linked factory.
        /// </summary>
        /// <returns>A new instance of the type <typeparamref name="T"/> created by the linked factory.</returns>
        protected override T NewInstance()
        {
            return Factory.Create();
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/> using the provided factory and appends it to the factory chain.
        /// This allows subsequent calls to create new instances as needed.
        /// </summary>
        /// <param name="factory">The factory to use for creating the object.</param>
        /// <returns>The object created by the factory.</returns>
        public static T CreateNew(IFactory<T> factory)
        {
            var factoryAppender = new FactoryAppender<T>();
            factoryAppender.LinkTo(factory);
            return factoryAppender.Create();
        }
    }
}

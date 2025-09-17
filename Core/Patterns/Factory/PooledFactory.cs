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

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// A factory that creates and manages instances of a specified type <typeparamref name="T"/> based on an index, 
    /// supporting object pooling and reusing existing instances when available.
    /// </summary>
    /// <typeparam name="T">The type of object that the factory creates.</typeparam>
    /// <typeparam name="TIndex">The type of the index used to manage the pooled instances.</typeparam>
    public class PooledFactory<T, TIndex> : FactoryAppender<T>, IPooledFactory<T, TIndex>
    {
        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> and adds it to the internal dictionary.
        /// If an instance with the specified index already exists, it is returned.
        /// </summary>
        /// <returns>A new instance of the type <typeparamref name="T"/>.</returns>
        public override T Create()
        {
            return Create(default(TIndex));
        }

        /// <summary>
        /// Creates a new instance of type <typeparamref name="T"/> based on the specified index.
        /// If an instance with the same index already exists, the existing instance is returned.
        /// Otherwise, a new instance is created and added to the internal dictionary.
        /// </summary>
        /// <param name="index">The index of the instance to retrieve or create.</param>
        /// <returns>The instance of type <typeparamref name="T"/> associated with the specified index.</returns>
        public virtual T Create(TIndex index)
        {
            if (!Instances.TryGetValue(index, out T result))
            {
                if (result == null)
                    result = NewInstance();
                Instances[index] = result;
            }

            return result;
        }

        /// <summary>
        /// A dictionary that holds pooled instances of type <typeparamref name="T"/> indexed by <typeparamref name="TIndex"/>.
        /// </summary>
        protected static ConcurrentDictionary<TIndex, T> Instances { get; set; } = new ConcurrentDictionary<TIndex, T>();

        /// <summary>
        /// Creates a new instance of the type <typeparamref name="T"/> using the provided factory and adds it to the internal pool.
        /// The instance is stored in a <see cref="ConcurrentDictionary"/> using an index of type <typeparamref name="TIndex"/>.
        /// </summary>
        /// <param name="factory">The factory to use for creating the object.</param>
        /// <returns>A new instance of the type <typeparamref name="T"/> created by the factory.</returns>
        public new static T CreateNew(IFactory<T> factory)
        {
            return CreateNew(factory, default(TIndex));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PooledFactory{T, TIndex}"/> and links it to the provided factory.
        /// Then, it calls the <see cref="Create"/> method on the new instance, passing in the specified index.
        /// </summary>
        /// <param name="factory">The factory to use for creating the object.</param>
        /// <param name="index">The index used to identify the specific object to create.</param>
        /// <returns>The object created by the factory.</returns>
        public static T CreateNew(IFactory<T> factory, TIndex index)
        {
            var factoryAppender = new PooledFactory<T, TIndex>();
            factoryAppender.LinkTo(factory);
            return factoryAppender.Create(index);
        }
    }
}
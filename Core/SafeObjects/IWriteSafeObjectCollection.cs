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
using System;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.SafeObjects
{
    public interface IWriteSafeObjectCollection<TKey, TValue, TType>: IWriteRepository<TValue, TKey>
        where TValue : ISafeObject<TType>
    {

        /// <summary>
        /// The try set value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result.</returns>
        bool TrySetValue(TKey key, object value);

        /// <summary>
        /// Gets or Sets the this[].
        /// </summary>
        new object this[TKey key] { get; set; }

        /// <summary>
        /// Clear the collection
        /// </summary>
        void Clear();

        ///// <summary>
        ///// Inserts a new entity.
        ///// </summary>
        ///// <param name="entity">Inserted entity</param>
        //void Insert(TKey id, TValue entity);

        ///// <summary>
        ///// Deletes an entity by primary key.
        ///// </summary>
        ///// <param name="id">Primary key of the entity</param>
        //void Delete(TKey id);

        ///// <summary>
        ///// Updates an existing entity.
        ///// </summary>
        ///// <param name="id">Id of the entity</param>
        ///// <param name="updateAction">Action that can be used to change values of the entity</param>
        ///// <returns>Updated entity</returns>
        //TValue Update(TKey id, Action<TValue> updateAction);
    }
}

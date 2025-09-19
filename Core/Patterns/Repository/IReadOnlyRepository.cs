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
using System.Collections.Generic;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Simple repository pattern
    /// </summary>
    public interface IReadOnlyRepository<TEntity, TPrimaryKey>
    {
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        TEntity Get(TPrimaryKey id);

        /// <summary>
        /// Used to get a IList that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>Ilist to be used to select entities</returns>
        IList<TEntity> GetAll();

        /// <summary>
        /// Query if primary key exists.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        bool Exists(TPrimaryKey id);

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        int Count();
    }
}

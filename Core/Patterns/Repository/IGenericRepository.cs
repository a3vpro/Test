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
using System.Linq;
using System.Linq.Expressions;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// A simple repository pattern interface for basic data operations.
    /// </summary>
    /// <typeparam name="T">The type of entity this repository manages</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">The entity to be inserted</param>
        void Insert(T entity);

        /// <summary>
        /// Deletes an existing entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted</param>
        void Delete(T entity);

        /// <summary>
        /// Searches for entities that match the given predicate.
        /// </summary>
        /// <param name="predicate">A condition used to filter entities</param>
        /// <returns>An IQueryable of entities that match the predicate</returns>
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieves all entities from the repository.
        /// </summary>
        /// <returns>An IQueryable of all entities</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity</param>
        /// <returns>The entity with the specified identifier</returns>
        T GetById(int id);
    }

}

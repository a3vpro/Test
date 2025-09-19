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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// This interface is implemented by all repositories to ensure implementation of fixed methods.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that the repository works with</typeparam>
    /// <typeparam name="TPrimaryKey">The primary key type of the entity</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : ISimpleRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Retrieves an IQueryable to access the entire set of entities, including specified related entities.
        /// </summary>
        /// <param name="propertySelectors">A list of expressions that specify related entities to include</param>
        /// <returns>An IQueryable to be used to select entities from the database</returns>
        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        /// <summary>
        /// Retrieves a list of all entities.
        /// </summary>
        /// <returns>A list containing all entities</returns>
        IList<TEntity> GetAllList();

        /// <summary>
        /// Retrieves a list of all entities that match a given predicate.
        /// </summary>
        /// <param name="predicate">A condition used to filter entities</param>
        /// <returns>A list of entities that match the condition</returns>
        IList<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously retrieves a list of all entities.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The result is a list of all entities</returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// Asynchronously retrieves a list of all entities that match a given predicate.
        /// </summary>
        /// <param name="predicate">A condition used to filter entities</param>
        /// <returns>A task that represents the asynchronous operation. The result is a list of entities that match the condition</returns>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Executes a query on the entities using a specified query method.
        /// </summary>
        /// <typeparam name="T">The return type of the query method</typeparam>
        /// <param name="queryMethod">A function to query the entities</param>
        /// <returns>The result of the query</returns>
        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

        /// <summary>
        /// Retrieves an IQueryable to access the entire set of entities.
        /// </summary>
        /// <returns>An IQueryable to be used to select entities from the database</returns>
        IQueryable<TEntity> Query();

        /// <summary>
        /// Asynchronously retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity</param>
        /// <returns>A task that represents the asynchronous operation. The result is the entity with the specified primary key</returns>
        Task<TEntity> GetAsync(TPrimaryKey id);

        /// <summary>
        /// Retrieves exactly one entity that matches a given predicate. 
        /// Throws an exception if no entities or more than one entity is found.
        /// </summary>
        /// <param name="predicate">A condition used to filter entities</param>
        /// <returns>The entity that matches the predicate</returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously retrieves exactly one entity that matches a given predicate. 
        /// Throws an exception if no entities or more than one entity is found.
        /// </summary>
        /// <param name="predicate">A condition used to filter entities</param>
        /// <returns>A task that represents the asynchronous operation. The result is the entity that matches the predicate</returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Retrieves an entity by its primary key or returns null if no entity is found.
        /// </summary>
        /// <param name="id">The primary key of the entity</param>
        /// <returns>The entity with the specified primary key, or null if no entity is found</returns>
        TEntity FirstOrDefault(TPrimaryKey id);

        /// <summary>
        /// Asynchronously retrieves an entity by its primary key or returns null if no entity is found.
        /// </summary>
        /// <param name="id">The primary key of the entity</param>
        /// <returns>A task that represents the asynchronous operation. The result is the entity with the specified primary key, or null if no entity is found</returns>
        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);

        /// <summary>
        /// Retrieves an entity that matches a given predicate or returns null if no entity is found.
        /// </summary>
        /// <param name="predicate">A condition used to filter entities</param>
        /// <returns>The entity that matches the predicate, or null if no entity is found</returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously retrieves an entity that matches a given predicate or returns null if no entity is found.
        /// </summary>
        /// <param name="predicate">A condition used to filter entities</param>
        /// <returns>A task that represents the asynchronous operation. The result is the entity that matches the predicate, or null if no entity is found</returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Creates an entity with the given primary key without accessing the database.
        /// </summary>
        /// <param name="id">The primary key of the entity</param>
        /// <returns>The created entity</returns>
        TEntity Load(TPrimaryKey id);

        /// <summary>
        /// Asynchronously inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">The entity to be inserted</param>
        /// <returns>A task that represents the asynchronous operation. The result is the inserted entity</returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// Inserts a new entity and returns its primary key.
        /// May require saving the current unit of work to retrieve the ID.
        /// </summary>
        /// <param name="entity">The entity to be inserted</param>
        /// <returns>The primary key of the inserted entity</returns>
        TPrimaryKey InsertAndGetId(TEntity entity);

        /// <summary>
        /// Asynchronously inserts a new entity and returns its primary key.
        /// May require saving the current unit of work to retrieve the ID.
        /// </summary>
        /// <param name="entity">The entity to be inserted</param>
        /// <returns>A task that represents the asynchronous operation. The result is the primary key of the inserted entity</returns>
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        /// <summary>
        /// Asynchronously updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to be updated</param>
        /// <returns>A task that represents the asynchronous operation. The result is the updated entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Asynchronously updates an existing entity based on its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to be updated</param>
        /// <param name="updateAction">An action that modifies the entity's properties</param>
        /// <returns>A task that represents the asynchronous operation. The result is the updated entity</returns>
        Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be deleted</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Asynchronously deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be deleted</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Asynchronously deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to be deleted</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAsync(TPrimaryKey id);

        /// <summary>
        /// Deletes entities that match the given predicate.
        /// This may cause performance issues if many entities are deleted.
        /// </summary>
        /// <param name="predicate">A condition used to filter the entities to delete</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously deletes entities that match the given predicate.
        /// This may cause performance issues if many entities are deleted.
        /// </summary>
        /// <param name="predicate">A condition used to filter the entities to delete</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously gets the count of all entities in the repository.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The result is the count of entities</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Gets the count of entities that match the given predicate.
        /// </summary>
        /// <param name="predicate">A condition used to filter the entities</param>
        /// <returns>The count of entities that match the predicate</returns>
        int Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously gets the count of entities that match the given predicate.
        /// </summary>
        /// <param name="predicate">A condition used to filter the entities</param>
        /// <returns>A task that represents the asynchronous operation. The result is the count of entities</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets the total count of all entities in the repository, even if it exceeds <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>The count of all entities in the repository</returns>
        long LongCount();

        /// <summary>
        /// Asynchronously gets the total count of all entities in the repository, even if it exceeds <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The result is the count of all entities</returns>
        Task<long> LongCountAsync();

        /// <summary>
        /// Gets the total count of entities that match the given predicate, even if it exceeds <see cref="int.MaxValue"/>.
        /// </summary>
        /// <param name="predicate">A condition used to filter the entities</param>
        /// <returns>The count of entities that match the predicate</returns>
        long LongCount(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously gets the total count of entities that match the given predicate, even if it exceeds <see cref="int.MaxValue"/>.
        /// </summary>
        /// <param name="predicate">A condition used to filter the entities</param>
        /// <returns>A task that represents the asynchronous operation. The result is the count of entities that match the predicate</returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
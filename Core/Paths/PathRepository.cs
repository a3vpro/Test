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
using VisionNet.Core.Patterns;

namespace VisionNet.IO.Paths
{
    /// <summary>
    /// Represents a repository for managing paths with associated IDs.
    /// Implements the ISimpleRepository interface for handling basic CRUD operations.
    /// </summary>
    public class PathRepository : ISimpleRepository<string, string>
    {
        private static Dictionary<string, string> _paths = new Dictionary<string, string>();

        /// <summary>
        /// Gets the count of paths in the repository.
        /// </summary>
        /// <returns>The number of paths in the collection.</returns>
        public int Count()
        {
            return _paths.Count();
        }

        /// <summary>
        /// Deletes a path from the repository based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the path to remove.</param>
        public void Delete(string id)
        {
            _paths.Remove(id);
        }

        /// <summary>
        /// Checks if a path with the specified ID exists in the repository.
        /// </summary>
        /// <param name="id">The ID of the path to check.</param>
        /// <returns>True if the path exists, otherwise false.</returns>
        public bool Exists(string id)
        {
            return _paths.ContainsKey(id);
        }

        /// <summary>
        /// Retrieves the path associated with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the path to retrieve.</param>
        /// <returns>The path associated with the given ID.</returns>
        public string Get(string id)
        {
            return _paths[id];
        }

        /// <summary>
        /// Retrieves all paths in the repository.
        /// </summary>
        /// <returns>A list of all paths.</returns>
        public IList<string> GetAll()
        {
            return _paths.Values.ToList();
        }

        /// <summary>
        /// Inserts a new path into the repository with the specified ID.
        /// If the ID already exists, it updates the existing path.
        /// </summary>
        /// <param name="id">The ID of the path to insert.</param>
        /// <param name="entity">The path associated with the ID.</param>
        public void Insert(string id, string entity)
        {
            _paths[id] = entity;
        }

        /// <summary>
        /// Updates the path associated with the specified ID using the provided update action.
        /// Throws a KeyNotFoundException if the ID does not exist in the repository.
        /// </summary>
        /// <param name="id">The ID of the path to update.</param>
        /// <param name="updateAction">The action to update the path.</param>
        /// <returns>The updated path.</returns>
        public string Update(string id, Func<string, string> updateAction)
        {
            if (!Exists(id))
                throw new KeyNotFoundException($"{id} not found in paths");

            var entity = _paths[id];
            entity = updateAction?.Invoke(entity);
            _paths[id] = entity;

            return entity;
        }
    }
}
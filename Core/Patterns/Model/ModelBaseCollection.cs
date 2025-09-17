//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 28-08-2021
// Description      : v1.0.0
//
// Copyright        : (C)  2021 by Sothis. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Provides a base observable collection that materializes view-model wrappers from a domain collection while
    /// maintaining access to the underlying domain objects.
    /// </summary>
    /// <typeparam name="TVM">Type of the view-model wrapper that exposes the domain object.</typeparam>
    /// <typeparam name="TDm">Type of the domain model instances contained in the collection.</typeparam>
    public abstract class ModelBaseCollection<TVM, TDm> : ObservableCollection<TVM>
    {
        #region Fields
        /// <summary>
        /// Backing storage for the exposed domain objects; kept in sync with <see cref="DomainCollection"/> so callers can
        /// inspect the underlying domain instances that generated the view-model items.
        /// </summary>
        public List<TDm> _mDomainCollection;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBaseCollection{TVM, TDm}"/> class without pre-populating
        /// domain or view-model elements.
        /// </summary>
        protected ModelBaseCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBaseCollection{TVM, TDm}"/> class by wrapping each supplied
        /// domain object in a <typeparamref name="TVM"/> instance and storing the corresponding domain items.
        /// </summary>
        /// <param name="domainCollection">Sequence of domain models that must be wrapped by <typeparamref name="TVM"/>.</param>
        /// <exception cref="MissingMethodException">
        /// Thrown when <typeparamref name="TVM"/> does not provide a public constructor that accepts a
        /// <typeparamref name="TDm"/> argument.
        /// </exception>
        protected ModelBaseCollection(IEnumerable<TDm> domainCollection)
        {
            // Set the domain collection
            _mDomainCollection = domainCollection.ToList();

            foreach (var domainObject in domainCollection)
            {
                var paramList = new object[] { domainObject };
                var wrapperObject = (TVM)Activator.CreateInstance(typeof(TVM), paramList);
                Add(wrapperObject);
            }
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Gets the current list of domain objects associated with the observable collection, exposing the same instances
        /// stored in <see cref="_mDomainCollection"/> so consumers can operate on the underlying data when required.
        /// </summary>
        public IList<TDm> DomainCollection { get { return _mDomainCollection; } }
        #endregion
    }
}

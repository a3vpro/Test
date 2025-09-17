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
    public abstract class ModelBaseCollection<TVM, TDm> : ObservableCollection<TVM>
    {
        #region Fields
        public List<TDm> _mDomainCollection;
        #endregion

        #region Constructor
        
        /// <summary> The ModelBaseCollection function is a constructor that initializes the ModelBaseCollection class.</summary>
        /// <returns> A new instance of the modelbasecollection class.</returns>
        protected ModelBaseCollection()
        {
        }

        /// <summary> The ModelBaseCollection function is a constructor that takes in an IEnumerable of domain objects and creates a wrapper object for each one.
        /// The wrapper object is then added to the collection.</summary>
        /// <param name="IEnumerable&lt;TDm&gt; domainCollection"> What is this parameter used for?</param>
        /// <returns> A modelbasecollection</returns>
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
        public IList<TDm> DomainCollection { get { return _mDomainCollection; } }
        #endregion
    }
}

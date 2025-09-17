using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Adapter that converts between <see cref="List{T}"/> and <see cref="ObservableCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class ObservableCollectionAdapter<T> : IBidirectionalAdapter<List<T>, ObservableCollectionWithItemChanges<T>>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Converts a list to an observable collection.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <returns>A new <see cref="ObservableCollection{T}"/> containing the items from the list.</returns>
        public ObservableCollectionWithItemChanges<T> Convert(List<T> source)
        {
            return new ObservableCollectionWithItemChanges<T>(source ?? Enumerable.Empty<T>());
        }

        /// <summary>
        /// Converts an observable collection to a list.
        /// </summary>
        /// <param name="source">The source observable collection.</param>
        /// <returns>A new <see cref="List{T}"/> containing the items from the observable collection.</returns>
        public List<T> Convert(ObservableCollectionWithItemChanges<T> source)
        {
            return source?.ToList() ?? new List<T>();
        }
    }
}

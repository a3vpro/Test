using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Provides extension methods to convert between <see cref="List{T}"/> and <see cref="ObservableCollection{T}"/>
    /// using <see cref="ObservableCollectionAdapter{T}"/>.
    /// </summary>
    public static class ObservableCollectionAdapterExtension
    {
        /// <summary>
        /// Converts a <see cref="List{T}"/> to an <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="list">The source list.</param>
        /// <returns>A new <see cref="ObservableCollection{T}"/> with the list's elements.</returns>
        public static ObservableCollectionWithItemChanges<T> ToObservableCollection<T>(this List<T> list)
            where T : INotifyPropertyChanged
        {
            var adapter = new ObservableCollectionAdapter<T>();
            return adapter.Convert(list);
        }

        /// <summary>
        /// Converts an <see cref="ObservableCollection{T}"/> to a <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="collection">The source observable collection.</param>
        /// <returns>A new <see cref="List{T}"/> with the collection's elements.</returns>
        public static List<T> ToSimpleList<T>(this ObservableCollectionWithItemChanges<T> collection)
            where T : INotifyPropertyChanged
        {
            var adapter = new ObservableCollectionAdapter<T>();
            return adapter.Convert(collection);
        }
    }
}

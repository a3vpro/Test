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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// An implementation of an observable collection that contains a duplicate internal
    /// list retained momentarily after the list is cleared. This allows observers to undo events
    /// on the list after it has been cleared and raises a <see cref="CollectionChanged"/> event with a Reset action.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class ObservableCollectionWithItemChanges<T> : ObservableCollection<T>, ICloneable where T : INotifyPropertyChanged
    {
        /// <summary>
        /// The inner list that retains items temporarily after a reset action.
        /// </summary>
        private readonly List<T> _inner = new List<T>();

        /// <summary>
        /// Set to 'true' when processing a collection changed event.
        /// </summary>
        private bool _inCollectionChangedEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionWithItemChanges{T}"/> class.
        /// </summary>
        public ObservableCollectionWithItemChanges()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionWithItemChanges{T}"/> class using the specified range of items.
        /// </summary>
        /// <param name="range">The range of items to initialize the collection with.</param>
        public ObservableCollectionWithItemChanges(IEnumerable<T> range) : base(range)
        {
            foreach (var item in range)
            {
                if (item is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionWithItemChanges{T}"/> class using the specified list.
        /// </summary>
        /// <param name="list">The list of items to initialize the collection with.</param>
        public ObservableCollectionWithItemChanges(IList<T> list) : base(list)
        {
            _inner.AddRange(list);
            foreach (var item in list)
            {
                if (item is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <param name="range">The range of items to add.</param>
        public void AddRange(T[] range)
        {
            foreach (var item in range)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <param name="range">The range of items to add.</param>
        public void AddRange(IEnumerable range)
        {
            foreach (T item in range)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <param name="range">The collection of items to add.</param>
        public void AddRange(ICollection<T> range)
        {
            foreach (var item in range)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes a range of items from the collection.
        /// </summary>
        /// <param name="range">The range of items to remove.</param>
        public void RemoveRange(T[] range)
        {
            foreach (var item in range)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Removes a range of items from the collection.
        /// </summary>
        /// <param name="range">The range of items to remove.</param>
        public void RemoveRange(IEnumerable range)
        {
            foreach (T item in range)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Removes a range of items from another collection.
        /// </summary>
        /// <param name="range">The range of items to remove from the collection.</param>
        public void RemoveRange(ObservableCollectionWithItemChanges<T> range)
        {
            foreach (var item in range)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Removes a range of items from the collection starting from a specific index.
        /// </summary>
        /// <param name="index">The index of the first item to be removed.</param>
        /// <param name="count">The number of items to remove.</param>
        public void RemoveRangeAt(int index, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes a range of items from the collection.
        /// </summary>
        /// <param name="range">The collection of items to remove.</param>
        public void RemoveRange(ICollection<T> range)
        {
            foreach (var item in range)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Removes a range of items from the collection.
        /// </summary>
        /// <param name="range">The range of items to remove from the collection.</param>
        public void RemoveRange(ICollection range)
        {
            foreach (T item in range)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Handles collection changes, updates the inner list, and raises appropriate events for added or removed items.
        /// </summary>
        /// <param name="e">The event arguments containing information about the change.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Trace.Assert(!_inCollectionChangedEvent);

            base.OnCollectionChanged(e);

            _inCollectionChangedEvent = true;

            try
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (_inner.Count > 0)
                    {
                        OnItemsRemoved(_inner);
                    }

                    _inner.Clear();
                }

                if (e.OldItems != null)
                {
                    foreach (T item in e.OldItems)
                    {
                        _inner.Remove(item);
                        if (item is INotifyPropertyChanged npc)
                        {
                            npc.PropertyChanged -= ItemPropertyChanged;
                        }
                    }

                    OnItemsRemoved(e.OldItems);
                    OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                }

                if (e.NewItems != null)
                {
                    foreach (T item in e.NewItems)
                    {
                        _inner.Add(item);
                        if (item is INotifyPropertyChanged npc)
                        {
                            npc.PropertyChanged += ItemPropertyChanged;
                        }
                    }

                    OnItemsAdded(e.NewItems);
                    OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                }
            }
            finally
            {
                _inCollectionChangedEvent = false;
            }
        }

        /// <summary>
        /// Invoked when items are added to the collection.
        /// </summary>
        protected virtual void OnItemsAdded(ICollection items) => ItemsAdded?.Invoke(this, new CollectionItemsChangedEventArgs(items));

        /// <summary>
        /// Invoked when items are removed from the collection.
        /// </summary>
        protected virtual void OnItemsRemoved(ICollection items) => ItemsRemoved?.Invoke(this, new CollectionItemsChangedEventArgs(items));

        /// <summary>
        /// Event raised when items have been added to the collection.
        /// </summary>
        public event EventHandler<CollectionItemsChangedEventArgs> ItemsAdded;

        /// <summary>
        /// Event raised when items have been removed from the collection.
        /// </summary>
        public event EventHandler<CollectionItemsChangedEventArgs> ItemsRemoved;

        /// <summary>
        /// Converts the collection to an array.
        /// </summary>
        /// <returns>An array containing all the elements in the collection.</returns>
        public T[] ToArray() => _inner.ToArray();

        /// <summary>
        /// Converts the collection to an array of a specified type.
        /// </summary>
        /// <typeparam name="T2">The type of the elements in the array.</typeparam>
        /// <returns>An array containing all the elements in the collection, cast to the specified type.</returns>
        public T2[] ToArray<T2>() where T2 : class
        {
            var array = new T2[Count];
            var i = 0;
            foreach (var obj in this)
            {
                array[i] = obj as T2;
                ++i;
            }

            return array;
        }

        /// <summary>
        /// Creates a clone of the collection, copying all elements from this collection.
        /// </summary>
        /// <returns>A new <see cref="ObservableCollectionWithItemChanges{T}"/> with the same elements as the current collection.</returns>
        public object Clone()
        {
            var clone = new ObservableCollectionWithItemChanges<T>();
            foreach (var unknown in this)
            {
                var obj = (ICloneable)unknown;
                clone.Add((T)obj.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Refreshes the collection by raising a reset collection change event for each item.
        /// </summary>
        public void Refresh()
        {
            for (var i = 0; i < Count; i++)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Solo notificamos si un cambio ha ocurrido en una propiedad
            if (sender is T item)
            {
                // Localizamos el índice del elemento en la colección
                int index = IndexOf(item);

                // Solo notificamos si el elemento existe en la colección
                if (index >= 0)
                {
                    OnPropertyChanged(new PropertyChangedEventArgs($"[{index}].{e.PropertyName}"));
                }
            }
        }

        // Implementación explícita de INotifyPropertyChanged para ReactiveUI
        /// <summary>
        /// Occurs when the collection reports a property change, either because the number of
        /// items has changed or because an item within the collection has raised a property
        /// change that is re-emitted with the item's index. The event arguments provide the
        /// property name that changed, such as <c>"Count"</c> or <c>"[index].Property"</c>.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged;

        // Método modificado para ser protected y virtual
        protected virtual new void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
            base.OnPropertyChanged(e);
        }
    }
}
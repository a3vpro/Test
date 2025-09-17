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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Provides a dictionary for use with data binding that raises collection and property change notifications.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in this collection.</typeparam>
    /// <typeparam name="TValue">The type of the values in this collection.</typeparam>
    [DebuggerDisplay("Count={Count}")]
    public class ObservableDictionary<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IDictionary,
        INotifyCollectionChanged, INotifyPropertyChanged
    {
        readonly IDictionary<TKey, TValue> dictionary;

        /// <summary>
        /// Event raised when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

        /// <summary>
        /// Event raised when a property on the collection changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        /// <summary>
        /// Initializes an instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
        /// </summary>
        public ObservableDictionary()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class using another dictionary as 
        /// the key/value store.
        /// </summary>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary, and raises a collection changed event with an Add action.
        /// Also raises PropertyChanged events for Count, Keys, and Values.
        /// </summary>
        /// <param name="item">The key-value pair to add.</param>
        void AddWithNotification(KeyValuePair<TKey, TValue> item)
        {
            AddWithNotification(item.Key, item.Value);
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary, then raises the CollectionChanged event with an Add action.
        /// Also raises PropertyChanged events for Count, Keys, and Values.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        void AddWithNotification(TKey key, TValue value)
        {
            dictionary.Add(key, value);

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                new KeyValuePair<TKey, TValue>(key, value)));
            PropertyChanged(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
            PropertyChanged(this, new PropertyChangedEventArgs("Values"));
        }

        /// <summary>
        /// Removes a key-value pair from the dictionary and raises the appropriate events.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>A boolean indicating whether the item was successfully removed.</returns>
        bool RemoveWithNotification(TKey key)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value) && dictionary.Remove(key))
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    new KeyValuePair<TKey, TValue>(key, value)));
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));
                PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                PropertyChanged(this, new PropertyChangedEventArgs("Values"));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the dictionary with a new value for an existing key.
        /// If the key does not exist, a new entry is added to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to update.</param>
        /// <param name="value">The new value for the element.</param>
        void UpdateWithNotification(TKey key, TValue value)
        {
            TValue existing;
            if (dictionary.TryGetValue(key, out existing))
            {
                dictionary[key] = value;

                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    new KeyValuePair<TKey, TValue>(key, value),
                    new KeyValuePair<TKey, TValue>(key, existing)));
                PropertyChanged(this, new PropertyChangedEventArgs("Values"));
            }
            else
            {
                AddWithNotification(key, value);
            }
        }

        /// <summary>
        /// Allows derived classes to raise custom property changed events.
        /// </summary>
        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged(this, args);
        }

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            AddWithNotification(key, value);
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey, TValue}"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary{TKey, TValue}"/>.</param>
        /// <returns>true if the <see cref="IDictionary{TKey, TValue}"/> contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Gets a collection containing the keys of the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false.</returns>
        public bool Remove(TKey key)
        {
            return RemoveWithNotification(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">The value associated with the specified key, if found.</param>
        /// <returns>true if the key was found; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set { UpdateWithNotification(key, value); }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Adds a new item to the dictionary.
        /// </summary>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            AddWithNotification(item);
        }

        /// <summary>
        /// Clears the dictionary of all key-value pairs.
        /// </summary>
        public void Clear()
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear();

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
            PropertyChanged(this, new PropertyChangedEventArgs("Values"));
        }

        /// <summary>
        /// Checks whether the dictionary contains a specific key-value pair.
        /// </summary>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular index.
        /// </summary>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of key-value pairs in the collection.
        /// </summary>
        public int Count
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count; }
        }

        /// <summary>
        /// Indicates whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly; }
        }

        /// <summary>
        /// Removes the specified item from the dictionary.
        /// </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return RemoveWithNotification(item.Key);
        }
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
        }

        #endregion

        #region IDictionary Members

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return key is TKey tKey && dictionary.ContainsKey(tKey);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)dictionary).GetEnumerator();
        }

        bool IDictionary.IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        bool IDictionary.IsReadOnly => ((IDictionary)dictionary).IsReadOnly;

        ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;

        void IDictionary.Remove(object key)
        {
            if (key is TKey tKey)
            {
                RemoveWithNotification(tKey);
            }
        }

        ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

        object IDictionary.this[object key]
        {
            get
            {
                if (key is TKey)
                    return dictionary[(TKey)key];
                return null;
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)dictionary).CopyTo(array, index);
        }

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => ((ICollection)dictionary).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

        #endregion
    }
}
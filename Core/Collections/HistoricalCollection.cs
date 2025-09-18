using System;
using System.Collections;
using System.Collections.Generic;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Maintains a fixed-size, thread-safe history of elements where the most recent value occupies index zero
    /// and older values are shifted toward the end as new entries are inserted.
    /// </summary>
    /// <typeparam name="T">Type of elements tracked in the historical buffer.</typeparam>
    public class HistoricalCollection<T> : IEnumerable<T>
    {
        private readonly object _syncLock = new object();
        private readonly T[] _elements;

        private int _count;

        /// <summary>
        /// Gets or sets the index of the currently selected element within the history, or -1 when no element is selected.
        /// </summary>
        /// <value>A zero-based index for the selected item, or -1 to indicate no selection.</value>
        public int SelectedIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the currently selected element, returning <c>default(T)</c> when the selection is cleared.
        /// </summary>
        /// <value>The selected item when <see cref="SelectedIndex"/> is valid; otherwise <c>default(T)</c>.</value>
        public T Selected { get; set; } = default(T);

        /// <summary>
        /// Gets the fixed maximum number of historical items retained by the collection.
        /// </summary>
        /// <value>The total capacity available for storing elements.</value>
        public int Capacity { get; }

        /// <summary>
        /// Gets the number of elements currently stored in the history in a thread-safe manner.
        /// </summary>
        /// <value>The count of tracked elements, limited by <see cref="Capacity"/>.</value>
        public int Count
        {
            get
            {
                lock (_syncLock)
                {
                    return _count;
                }
            }
        }

        /// <summary>
        /// Initializes a new historical buffer with the specified maximum capacity.
        /// </summary>
        /// <param name="capacity">Fixed number of elements the collection can retain; must be greater than zero.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="capacity"/> is less than or equal to zero.</exception>
        public HistoricalCollection(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity should be greather than zero.", nameof(capacity));

            Capacity = capacity;
            _elements = new T[capacity];
            _count = 0;
        }

        /// <summary>
        /// Inserts a new element at the head of the history, shifting existing entries toward the tail and discarding the oldest when capacity is exceeded.
        /// </summary>
        /// <param name="item">Element to insert as the most recent entry.</param>
        public void Add(T item)
        {
            lock (_syncLock)
            {
                // Si no está llena, se incrementa _count.
                // Si está llena, se sobrescribe el último y se pierde el más antiguo.
                int insertPos = 0;

                if (_count < Capacity)
                {
                    // Desplazar [0.._count-1] hacia [1.._count]
                    for (int i = _count; i > 0; i--)
                    {
                        _elements[i] = _elements[i - 1];
                    }
                    _count++;
                }
                else
                {
                    // _count == Capacity
                    // Desplazar [0..Capacity-2] hacia [1..Capacity-1]
                    for (int i = Capacity - 1; i > 0; i--)
                    {
                        _elements[i] = _elements[i - 1];
                    }
                }

                // Insertar el nuevo elemento en la posición 0.
                _elements[insertPos] = item;

                // Ajustar el SelectedIndex si está seleccionado algo
                if (SelectedIndex != -1)
                {
                    SelectedIndex++;

                    // Si el índice seleccionado se sale de rango, se pierde la selección.
                    if (SelectedIndex >= _count)
                    {
                        SelectedIndex = -1;
                        Selected = default(T);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the most recent element stored in the history.
        /// </summary>
        /// <returns>The element at index zero, or <c>default(T)</c> when the collection is empty.</returns>
        public T Newest()
        {
            lock (_syncLock)
            {
                if (_count == 0)
                    return default(T);

                return _elements[0];
            }
        }

        /// <summary>
        /// Removes all stored elements and clears the current selection in a thread-safe manner.
        /// </summary>
        public void Clear()
        {
            lock (_syncLock)
            {
                Array.Clear(_elements, 0, _elements.Length);
                _count = 0;

                // También limpiamos la selección
                SelectedIndex = -1;
                Selected = default(T);
            }
        }

        /// <summary>
        /// Gets the element at the specified historical index, where zero represents the most recent entry.
        /// </summary>
        /// <param name="index">Zero-based offset within the history. Values outside the current range return <c>default(T)</c>.</param>
        /// <returns>The element stored at the requested position, or <c>default(T)</c> if the index is invalid.</returns>
        public T this[int index]
        {
            get
            {
                lock (_syncLock)
                {
                    if (index < 0 || index >= _count)
                        return default(T);

                    return _elements[index];
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a snapshot of the current historical elements.
        /// </summary>
        /// <returns>An enumerator yielding items from the most recent to the oldest as captured when enumeration begins.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            T[] snapshot;
            lock (_syncLock)
            {
                snapshot = new T[_count];
                for (int i = 0; i < _count; i++)
                {
                    snapshot[i] = _elements[i];
                }
            }

            foreach (T item in snapshot)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Returns a non-generic enumerator that iterates through the historical snapshot.
        /// </summary>
        /// <returns>An enumerator exposing the same snapshot provided by <see cref="GetEnumerator()"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

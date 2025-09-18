using System;
using System.Collections;
using System.Collections.Generic;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Maintains a fixed-size, thread-safe collection that tracks items in reverse chronological order,
    /// always placing the most recent entry at index 0 while shifting older entries toward the end.
    /// </summary>
    /// <typeparam name="T">Type of items captured in the historical record.</typeparam>
    public class HistoricalCollection<T> : IEnumerable<T>
    {
        private readonly object _syncLock = new object();
        private readonly T[] _elements;

        private int _count;

        /// <summary>
        /// Gets or sets the index of the selected element within the historical sequence; -1 indicates that no element is selected.
        /// </summary>
        public int SelectedIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the currently selected element; equals <see cref="default"/> when <see cref="SelectedIndex"/> equals -1 or the selected entry has been discarded.
        /// </summary>
        public T Selected { get; set; } = default(T);

        /// <summary>
        /// Gets the maximum number of elements that can be retained in the historical buffer.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Gets the current number of items stored in the collection in a thread-safe manner.
        /// </summary>
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
        /// Initializes a new instance of the <see cref="HistoricalCollection{T}"/> with a fixed capacity used to retain the most recent entries.
        /// </summary>
        /// <param name="capacity">Total number of elements that can be preserved before the oldest entries are discarded; must be greater than zero.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="capacity"/> is less than or equal to zero to prevent an unusable collection.</exception>
        public HistoricalCollection(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity should be greather than zero.", nameof(capacity));

            Capacity = capacity;
            _elements = new T[capacity];
            _count = 0;
        }

        /// <summary>
        /// Inserts an item as the most recent entry, shifting existing elements to older positions and dropping the oldest entry when the capacity is full.
        /// </summary>
        /// <param name="item">Item captured for historical tracking; may be <see cref="default"/>.</param>
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
        /// Retrieves the most recent item (index 0) currently stored in the history.
        /// Returns <see cref="default"/> when the collection is empty.
        /// </summary>
        /// <returns>The most recently added element, or <see cref="default"/> if no elements exist.</returns>
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
        /// Removes all items from the historical buffer and resets the selection state in a thread-safe manner.
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
        /// Gets the element at the specified index, where 0 represents the most recent entry and higher values represent progressively older entries.
        /// Returns <see cref="default"/> for out-of-range requests.
        /// </summary>
        /// <param name="index">Zero-based position within the historical sequence.</param>
        /// <returns>The element at <paramref name="index"/>, or <see cref="default"/> when the index is outside the tracked range.</returns>
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
        /// Creates an enumerator over a snapshot of the current items, preserving the newest-to-oldest ordering at the time of enumeration.
        /// </summary>
        /// <returns>An enumerator that iterates through a stable copy of the tracked history.</returns>
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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

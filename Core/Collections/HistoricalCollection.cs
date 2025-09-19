using System;
using System.Collections;
using System.Collections.Generic;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Represents a thread-safe, fixed-capacity history of elements where the most recently added item is stored at index 0
    /// and older items are shifted down, discarding the oldest when the capacity is exceeded. Selection metadata tracks a
    /// logical current item even as insertions reorder stored values.
    /// </summary>
    /// <remarks>
    /// The collection maintains a bounded timeline of items, always preserving order from newest to oldest. Enumeration returns
    /// a snapshot to avoid locking during iteration, and selection state is adjusted when insertions displace existing entries.
    /// </remarks>
    /// <typeparam name="T">Type of items stored in the historical collection.</typeparam>
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
        /// Gets or sets the selected element associated with <see cref="SelectedIndex"/>; equals <c>default</c> when nothing is selected.
        /// </summary>
        /// <value>The selected item when <see cref="SelectedIndex"/> is valid; otherwise <c>default(T)</c>.</value>
        public T Selected { get; set; } = default(T);

        /// <summary>
        /// Gets the fixed maximum number of elements that can be retained in the collection.
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
        /// <param name="item">Item to add to the historical sequence.</param>
        /// <remarks>
        /// When a selection is active, the selected index is advanced to maintain its association with the same logical entry;
        /// the selection is cleared if the tracked element falls outside the retained range.
        /// </remarks>
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
        /// Gets the element at the specified zero-based index, where index 0 represents the most recent item.
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
        /// Returns an enumerator that iterates over a snapshot of the current elements in order from newest to oldest.
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace VisionNet.Core.Collections
{
    public class HistoricalCollection<T> : IEnumerable<T>
    {
        private readonly object _syncLock = new object();
        private readonly T[] _elements;

        private int _count;

        /// <summary>
        /// Índice del elemento seleccionado. -1 indica que no hay ningún elemento seleccionado.
        /// </summary>
        public int SelectedIndex { get; set; } = -1;

        /// <summary>
        /// Elemento seleccionado (puede ser default(T) si SelectedIndex = -1).
        /// </summary>
        public T Selected { get; set; } = default(T);

        /// <summary>
        /// Capacidad máxima de la colección (tamaño fijo).
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Número actual de elementos en la colección (thread-safe).
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
        /// Crea la colección con una capacidad máxima fija (> 0).
        /// </summary>
        /// <param name="capacity">Capacidad máxima.</param>
        public HistoricalCollection(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity should be greather than zero.", nameof(capacity));

            Capacity = capacity;
            _elements = new T[capacity];
            _count = 0;
        }

        /// <summary>
        /// Agrega un nuevo elemento en la posición 0 (la más "reciente").
        /// Si la colección está llena, se elimina el elemento más antiguo (al final).
        /// </summary>
        /// <param name="item">Elemento a agregar.</param>
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
        /// Devuelve el elemento más reciente (índice 0).
        /// Lanza excepción si no hay elementos.
        /// </summary>
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
        /// Limpia la colección y la selección (thread-safe).
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
        /// Indizador para obtener el elemento por índice (0 = más reciente).
        /// </summary>
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
        /// Permite la iteración con foreach, retornando una instantánea de los elementos.
        /// </summary>
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

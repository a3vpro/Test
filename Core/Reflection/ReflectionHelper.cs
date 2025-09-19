using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VisionNet.Core.Reflection
{
    /// <summary>
    /// Provides helpers for interacting with non-public members through reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Creates a <see cref="List{T}"/> instance for the specified element type using reflection and optionally populates it with a default value.
        /// </summary>
        /// <param name="elementType">The element <see cref="Type"/> to use when constructing the list. Must be compatible with <paramref name="defaultValue"/>.</param>
        /// <param name="count">The number of times to repeat <paramref name="defaultValue"/> in the resulting list. Must be zero or greater.</param>
        /// <param name="defaultValue">An optional default value inserted repeatedly in the list. The value must be assignable to <paramref name="elementType"/> when not <see langword="null"/>.</param>
        /// <returns>An <see cref="IList"/> whose runtime type is <see cref="List{T}"/> of <paramref name="elementType"/>, optionally pre-populated with <paramref name="defaultValue"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="defaultValue"/> is not <see langword="null"/> and is not assignable to <paramref name="elementType"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="elementType"/> is <see langword="null"/>.</exception>
        /// <remarks>The list type is materialized by calling <see cref="Type.MakeGenericType(Type[])"/> on <see cref="List{T}"/> followed by <see cref="Activator.CreateInstance(Type)"/>.</remarks>
        public static IList CreateListOfType(Type elementType, int count = 0, object defaultValue = null)
        {
            // Crea una instancia de List<> del tipo especificado.
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            // Llena la lista con el valor por defecto especificado, la cantidad de veces indicada.
            for (int i = 0; i < count; i++)
            {
                // Asegura que el valor por defecto sea del tipo correcto o sea null.
                if (defaultValue == null || elementType.IsInstanceOfType(defaultValue))
                {
                    list.Add(defaultValue);
                }
                else
                {
                    throw new ArgumentException($"El valor por defecto no es del tipo {elementType.Name}.");
                }
            }

            return list;
        }

        /// <summary>
        /// Sets the value of an internal instance property using reflection.
        /// </summary>
        /// <param name="targetObject">The object whose internal property should be updated. Must not be <see langword="null"/>.</param>
        /// <param name="propertyName">The case-sensitive name of the internal instance property to modify.</param>
        /// <param name="newValue">The new value assigned to the property. The value must be assignable to the property's type.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="targetObject"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        /// <exception cref="TargetInvocationException">Thrown when the property's set accessor throws an exception.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newValue"/> is not compatible with the property type.</exception>
        /// <remarks>The property is located by combining <see cref="BindingFlags.NonPublic"/> and <see cref="BindingFlags.Instance"/>. If the property cannot be found or is not writable, this method does nothing.</remarks>
        public static void ModifyInternalProperty(object targetObject, string propertyName, object newValue)
        {
            // Obtén el tipo del objeto
            Type targetType = targetObject.GetType();

            // Encuentra la propiedad internal
            PropertyInfo property = targetType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                // Modifica el valor
                property.SetValue(targetObject, newValue);
            }
        }

        /// <summary>
        /// Sets the value of an internal static property using reflection.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/> that declares the internal static property. Must not be <see langword="null"/>.</param>
        /// <param name="propertyName">The case-sensitive name of the internal static property to modify.</param>
        /// <param name="newValue">The new value assigned to the property. The value must be assignable to the property's type.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        /// <exception cref="TargetInvocationException">Thrown when the property's set accessor throws an exception.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newValue"/> is not compatible with the property type.</exception>
        /// <remarks>The property is located by combining <see cref="BindingFlags.NonPublic"/> and <see cref="BindingFlags.Static"/>. If the property cannot be found or is not writable, this method does nothing.</remarks>
        public static void ModifyInternalStaticProperty(Type targetType, string propertyName, object newValue)
        {
            // Encuentra la propiedad estática internal
            PropertyInfo property = targetType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            if (property != null && property.CanWrite)
            {
                // Modifica el valor de la propiedad
                property.SetValue(null, newValue);
            }
        }

        /// <summary>
        /// Sets the value of an internal instance field using reflection.
        /// </summary>
        /// <param name="targetObject">The object whose internal field should be updated. Must not be <see langword="null"/>.</param>
        /// <param name="propertyName">The case-sensitive name of the internal instance field to modify.</param>
        /// <param name="newValue">The new value assigned to the field. The value must be assignable to the field's type.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="targetObject"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newValue"/> is not compatible with the field type.</exception>
        /// <remarks>The field is located by combining <see cref="BindingFlags.NonPublic"/> and <see cref="BindingFlags.Instance"/>. If the field cannot be found, this method does nothing.</remarks>
        public static void ModifyInternalField(object targetObject, string propertyName, object newValue)
        {
            // Obtén el tipo del objeto
            Type targetType = targetObject.GetType();

            // Encuentra la propiedad internal
            FieldInfo field = targetType.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                // Modifica el valor
                field.SetValue(targetObject, newValue);
            }
        }

        /// <summary>
        /// Sets the value of an internal static field using reflection.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/> that declares the internal static field. Must not be <see langword="null"/>.</param>
        /// <param name="propertyName">The case-sensitive name of the internal static field to modify.</param>
        /// <param name="newValue">The new value assigned to the field. The value must be assignable to the field's type.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="targetType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newValue"/> is not compatible with the field type.</exception>
        /// <remarks>The field is located by combining <see cref="BindingFlags.NonPublic"/> and <see cref="BindingFlags.Static"/>. If the field cannot be found, this method does nothing.</remarks>
        public static void ModifyInternalStaticField(Type targetType, string propertyName, object newValue)
        {
            // Encuentra la propiedad estática internal
            FieldInfo field = targetType.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                // Modifica el valor de la propiedad
                field.SetValue(null, newValue);
            }
        }
    }
}

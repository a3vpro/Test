using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VisionNet.Core.Reflection
{
    public static class ReflectionHelper
    {
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

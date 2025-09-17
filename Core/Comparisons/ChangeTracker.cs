using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VisionNet.Core.Comparisons
{
    public class ChangeTracker
    {
        private readonly ConcurrentBag<string> _changedProperties = new ConcurrentBag<string>();

        public IReadOnlyCollection<string> ChangedProperties => _changedProperties;

        public bool HasChanges => !_changedProperties.IsEmpty;

        public void Clear()
        {
            while (!_changedProperties.IsEmpty)
            {
                _changedProperties.TryTake(out string _);
            }
        }

        public void TrackDiff(object original, object modified, string prefix = "")
        {
            if (original == null && modified == null) return;
            if (original == null || modified == null || original.GetType() != modified.GetType())
            {
                TrackChange(prefix);
                return;
            }

            var type = original.GetType();

            // Comparar listas
            if (original is IList origList && modified is IList modList)
            {
                int min = System.Math.Min(origList.Count, modList.Count);
                for (int i = 0; i < min; i++)
                {
                    TrackDiff(origList[i], modList[i], $"{prefix}[{i}]");
                }

                if (origList.Count > modList.Count)
                {
                    for (int i = min; i < origList.Count; i++)
                        TrackChange($"{prefix}[{i}]"); // Eliminado
                }
                else if (modList.Count > origList.Count)
                {
                    for (int i = min; i < modList.Count; i++)
                        TrackChange($"{prefix}[{i}]"); // Añadido
                }

                return;
            }

            // Comparar diccionarios
            if (original is IDictionary origDict && modified is IDictionary modDict)
            {
                var keys = origDict.Keys.Cast<object>().Union(modDict.Keys.Cast<object>()).Distinct();
                foreach (var key in keys)
                {
                    var keyStr = key?.ToString() ?? "null";
                    bool hasOrig = origDict.Contains(key);
                    bool hasMod = modDict.Contains(key);

                    if (hasOrig && hasMod)
                    {
                        TrackDiff(origDict[key], modDict[key], $"{prefix}[{keyStr}]");
                    }
                    else
                    {
                        TrackChange($"{prefix}[{keyStr}]"); // Añadido o eliminado
                    }
                }

                return;
            }

            // Comparar propiedades públicas
            if (!type.IsPrimitive && type != typeof(string))
            {
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!prop.CanRead || prop.GetIndexParameters().Length > 0) continue;

                    // Comprobar si la propiedad debe ser trackeada
                    var trackAttr = prop.GetCustomAttribute<TrackChangesAttribute>(true);
                    if (trackAttr != null && !trackAttr.Track) continue;

                    var val1 = prop.GetValue(original);
                    var val2 = prop.GetValue(modified);

                    var path = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                    TrackDiff(val1, val2, path);
                }
                return;
            }

            // Fallback: comparación simple
            if (!Equals(original, modified))
            {
                TrackChange(prefix);
            }
        }

        public void TrackChange(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                _changedProperties.Add(propertyName);
            }
        }
    }
}

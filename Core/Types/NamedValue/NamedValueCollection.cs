using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VisionNet.Core.Types
{
    public class NamedValueCollection : INamedValueCollection
    {
        private readonly ConcurrentDictionary<string, NamedValue> _values = new ConcurrentDictionary<string, NamedValue>();

        public int Count()
        {
            return _values.Count;
        }

        public void Delete(string id)
        {
            _values.TryRemove(id, out var value);
        }

        public bool Exists(string id)
        {
            return _values.ContainsKey(id);
        }

        public NamedValue Get(string id)
        {
            _values.TryGetValue(id, out var entity);
            return entity;
        }

        public IList<NamedValue> GetAll()
        {
            return _values.Values.ToList();
        }

        public void Insert(string id, NamedValue entity)
        {
            _values.TryAdd(id, entity);
        }

        public NamedValue Update(string id, Func<NamedValue, NamedValue> updateAction)
        {
            NamedValue result = default(NamedValue);
            var success = _values.TryGetValue(id, out result);
            if (success)
            {
                result = updateAction(result);
                _values.AddOrUpdate(id, result, (i, e) => e);
            }
            return result;
        }
    }
}

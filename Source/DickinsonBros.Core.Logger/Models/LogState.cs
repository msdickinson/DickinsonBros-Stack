using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DickinsonBros.Core.Logger.Models
{
    internal class LogState : IReadOnlyList<KeyValuePair<string, object>>
    {
        internal readonly IList<KeyValuePair<string, object>> _keyValuePairs;

        public LogState(IList<KeyValuePair<string, object>> keyValuePairs)
        {
            _keyValuePairs = keyValuePairs;
        }

        public int Count
        {
            get
            {
                return _keyValuePairs == null ? 0 : _keyValuePairs.Count();
            }
        }

        public KeyValuePair<string, object> this[int index]
        {
            get
            {
                return new KeyValuePair<string, object>(_keyValuePairs[index].Key, _keyValuePairs[index].Value);
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Data
{
    public abstract class Database<T> : ScriptableObject, IEnumerable<T>
    {
        #region Serialized Fields
        [Header("Game data")]
        [SerializeField] private T[] _allData;
        #endregion

        #region IEnumerable Implementation
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T data in _allData)
                yield return data;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _allData.GetEnumerator();
        }
        #endregion
    }
}

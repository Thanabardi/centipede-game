using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Thanabardi.Generic.Core.StateSystem
{
    public sealed class ObjPool<T> : IEnumerable<T> where T : MonoBehaviour
    {
        private List<T> list;
        private Transform _parent;
        private T _prefab;

        public int Count => list.Count;

        public T this[int index] => list[index];

        public ObjPool(T prefab, Transform parent)
        {
            list = new List<T>();
            _prefab = prefab;
            _parent = parent;
        }

        public T Get()
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].gameObject.activeSelf)
                {
                    list[i].gameObject.SetActive(true);
                    return list[i];
                }
            }
            // expand pool size
            T item = GameObject.Instantiate(_prefab, _parent);
            item.gameObject.SetActive(true);
            list.Add(item);
            return item;
        }

        public void Remove(T item)
        {
            T foundItem = list.FirstOrDefault(i => i == item);
            foundItem?.gameObject.SetActive(false);
        }

        public void Delete(T item)
        {
            T foundItem = list.FirstOrDefault(i => i == item);
            if (foundItem != null)
            {
                GameObject.Destroy(foundItem.gameObject);
                list.Remove(foundItem);
            }
        }

        public void Clear()
        {
            foreach (var item in list)
            {
                if (item != null)
                    GameObject.Destroy(item.gameObject);
            }
            list.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
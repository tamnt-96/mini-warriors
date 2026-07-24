using System.Collections.Generic;
using UnityEngine;

namespace TWR.Core
{
    public class ObjectPool
    {
        readonly GameObject _prefab;
        readonly Transform _root;
        readonly Queue<GameObject> _pool = new();

        public ObjectPool(GameObject prefab, Transform root, int prewarm = 0)
        {
            _prefab = prefab;
            _root = root;
            for (int i = 0; i < prewarm; i++)
                _pool.Enqueue(CreateNew());
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            var obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(_root);
            _pool.Enqueue(obj);
        }

        GameObject CreateNew()
        {
            var obj = Object.Instantiate(_prefab, _root);
            obj.SetActive(false);
            return obj;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Codebase.Infrastructure.Services.Pooling
{
    public class ObjectPool<T> : IObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _objects = new();

        private readonly string _parentName;
        private readonly T _prefab;
        private readonly Action<T> _onCreateCallback;
        private readonly Action<T> _onGetCallback;
        private readonly Action<T> _onReleaseCallback;
        private readonly Action _onPoolInitializedCallback;
        private readonly int _size;
        private readonly int _maxPoolSize;
        private Transform _parent;

        public ObjectPool(string parentName, T prefab, Action<T> onCreateCallback = null,
            Action<T> onGetCallback = null, Action<T> onReleaseCallback = null, Action onPoolInitializedCallback = null,
            int size = 100, int maxPoolSize = 10000)
        {
            _parentName = parentName;
            _prefab = prefab;
            _onCreateCallback = onCreateCallback;
            _onGetCallback = onGetCallback;
            _onReleaseCallback = onReleaseCallback;
            _onPoolInitializedCallback = onPoolInitializedCallback;
            _size = size;
            _maxPoolSize = maxPoolSize;
            CreateParent(_parentName);
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < _size; i++)
            {
                _objects.Enqueue(InstantiateObject());
            }

            _onPoolInitializedCallback?.Invoke();
        }

        private void CreateParent(string parentName)
        {
            _parent = new GameObject(parentName + "_parent").transform;
            _parent.position = new Vector3(-200, -200, -200);
            Object.DontDestroyOnLoad(_parent.gameObject);
        }

        public T Get(Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            if (position == default) position = Vector3.zero;
            if (rotation == default) rotation = Quaternion.identity;

            var pooledObject = _objects.Count > 0 ? _objects.Dequeue() : InstantiateObject();

            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;
            pooledObject.transform.SetParent(parent);
            pooledObject.gameObject.SetActive(true);
            OnGetCallback(pooledObject);

            return pooledObject;
        }

        public void Release(T obj)
        {
            _objects.Enqueue(obj);
            obj.transform.parent = _parent;

            obj.gameObject.SetActive(false);
            OnReleaseCallback(obj);
        }

        private T InstantiateObject()
        {
            T instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            OnCreateCallback(instance);
            return instance;
        }

        private void OnCreateCallback(T createdObject)
        {
            _onCreateCallback?.Invoke(createdObject);
        }

        private void OnGetCallback(T givenObject)
        {
            (givenObject as IPoolableObject<T>).Construct(this);

            _onGetCallback?.Invoke(givenObject);

            (givenObject as IGetPooledObjectHandler).OnGet();
        }

        private void OnReleaseCallback(T releasedObject)
        {
            _onReleaseCallback?.Invoke(releasedObject);

            (releasedObject as IReleasePooledObjectHandler).OnRelease();
        }

        public void Clean()
        {
        }
    }
}
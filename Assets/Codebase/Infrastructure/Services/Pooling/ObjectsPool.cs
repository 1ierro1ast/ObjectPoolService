using System;
using System.Collections.Generic;
using Codebase.Infrastructure.Services.AssetManagement;
using UnityEngine;

#if UNITY_2021_1_OR_NEWER
using UnityEngine.Pool;
#endif

namespace Codebase.Infrastructure.Services.Pooling
{
    public class ObjectsPool<T> : IObjectsPool<T> where T : MonoBehaviour
    {
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _poolContainer;
        private readonly int _poolSize;
        private readonly int _maxPoolSize;
        private readonly T _prefab;
        private readonly Action<T> _actionOnGet;
        private readonly Action<T> _actionOnRelease;
        private readonly Queue<T> _pooledObjects;
#if UNITY_2021_1_OR_NEWER
        private readonly ObjectPool<T> _objectPool;
#endif

        public ObjectsPool(IAssetProvider assetProvider, T prefab, Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null, int poolSize = 100, int maxPoolSize = 1000)
        {
            _assetProvider = assetProvider;
            _poolSize = poolSize;
            _maxPoolSize = maxPoolSize;
            _prefab = prefab;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
#if UNITY_2021_1_OR_NEWER
            _objectPool =
                new ObjectPool<T>(CreatePooledObject, OnGetCallback, OnReleaseCallback, defaultCapacity: poolSize);
#else
            _poolContainer = new GameObject($"{typeof(T)}_PoolContainer").transform;
            _pooledObjects = new Queue<T>(_poolSize);
            CreatePool();
#endif
        }

        public T Get(Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
#if UNITY_2021_1_OR_NEWER
            var objectFromPool = _objectPool.Get();
#else
            var objectFromPool = _pooledObjects.Count > 0 ? _pooledObjects.Dequeue() : CreatePooledObject();
            var objectTransform = objectFromPool.transform;
            objectTransform.position = position;
            objectTransform.rotation = rotation;

            objectTransform.SetParent(parent);

            OnGetCallback(objectFromPool);

            objectFromPool.gameObject.SetActive(true);
#endif

            return objectFromPool;
        }

        public void Release(T releasingObject)
        {
#if UNITY_2021_1_OR_NEWER
            _objectPool.Release(releasingObject);
#else
             if (_pooledObjects.Count >= _maxPoolSize)
                throw new Exception(
                    $"It is not possible to put an object in the pool because the maximum pool size has been exceeded! Current maximum pool size: {_maxPoolSize}");
            OnReleaseCallback(releasingObject);

            releasingObject.transform.SetParent(_poolContainer);
            releasingObject.gameObject.SetActive(false);
            _pooledObjects.Enqueue(releasingObject);
#endif
        }

        private void OnGetCallback(T objectFromPool)
        {
            _actionOnGet?.Invoke(objectFromPool);
            
            if (objectFromPool is ICreatePooledObjectHandler)
                ((ICreatePooledObjectHandler) objectFromPool).OnCreate();
        }

        private void OnReleaseCallback(T objectForDisposing)
        {
            _actionOnRelease?.Invoke(objectForDisposing);
            
            if (objectForDisposing is IReleasePooledObjectHandler)
                ((IReleasePooledObjectHandler) objectForDisposing).OnRelease();
        }

        private void CreatePool()
        {
            for (int i = 0; i < _poolSize; i++)
                CreatePooledObject();
        }

        private T CreatePooledObject()
        {
            if (_pooledObjects.Count >= _maxPoolSize)
                throw new Exception(
                    $"It is impossible to create an object because the maximum pool size has been exceeded! Current maximum pool size: {_maxPoolSize}");
            
            var pooledObject = _assetProvider.Instantiate(_prefab, _poolContainer);
            pooledObject.gameObject.SetActive(false);
            _pooledObjects.Enqueue(pooledObject);
            return pooledObject;
        }
    }
}
using System.Collections.Generic;
using Codebase.Infrastructure.Services.AssetManagement;
using UnityEngine;

namespace Codebase.Infrastructure.Services.Pooling
{
    public class ObjectPool<T> : IObjectPool<T> where T : Component
    {
        private readonly IAssetProvider _assetProvider;
        private readonly T _objectPrefab;
        private Queue<T> _pooledObjects;
        private Transform _poolContainer;

        public ObjectPool(IAssetProvider assetProvider, int poolSize)
        {
            _assetProvider = assetProvider;
            _objectPrefab = _assetProvider.GetObject<T>(AssetPath.PooledObjectsPath);
            _poolContainer = _assetProvider.Instantiate<Transform>(AssetPath.PoolContainerPath);
            _poolContainer.name = $"{nameof(T)}_PoolContainer";
            _pooledObjects = new Queue<T>(poolSize);
        }

        private void Initialize()
        {
            for (int i = 0; i < 50; i++)
            {
                CreatePooledObject();
            }
        }

        private T CreatePooledObject()
        {
            var pooledObject = _assetProvider.Instantiate(_objectPrefab, _poolContainer);
            pooledObject.gameObject.SetActive(false);
            _pooledObjects.Enqueue(pooledObject);
            return pooledObject;
        }


        public T Create(Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var objectFromPool = _pooledObjects.Count > 0 ? _pooledObjects.Dequeue() : CreatePooledObject();
            
            var objectTransform = objectFromPool.transform;
            objectTransform.position = position;
            objectTransform.rotation = rotation;

            objectTransform.SetParent(parent);
            objectFromPool.gameObject.SetActive(true);
            return objectFromPool;
        }

        public void Dispose(T disposingObject)
        {
            disposingObject.transform.SetParent(_poolContainer);
            disposingObject.gameObject.SetActive(false);
            _pooledObjects.Enqueue(disposingObject);
        }
    }
}
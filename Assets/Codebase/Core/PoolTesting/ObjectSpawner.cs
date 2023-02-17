using Codebase.Infrastructure.Services.Pooling;
using UnityEngine;

namespace Codebase.Core.PoolTesting
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private TestPoolableObject _prefab;
        private ObjectPool<TestPoolableObject> _objectsPool;

        private void Awake()
        {
            _objectsPool = new ObjectPool<TestPoolableObject>("TestPool", _prefab, OnCreateCallback, OnGetCallback,
                OnReleaseCallback, OnPoolInitializedCallback,10, 100);
        }

        private void OnPoolInitializedCallback()
        {
            Debug.Log("Pool initialized");
        }

        private void OnReleaseCallback(TestPoolableObject obj)
        {
            Debug.Log($"Release object to pool {obj.GetType()}");
        }

        private void OnGetCallback(TestPoolableObject obj)
        {
            Debug.Log($"Get object from pool {obj.GetType()}");
        }

        private void OnCreateCallback(TestPoolableObject obj)
        {
            Debug.Log($"Create new object {obj.GetType()}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _objectsPool.Get(parent: transform);
            }
        }
    }
}
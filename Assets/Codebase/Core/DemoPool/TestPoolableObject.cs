using Codebase.Infrastructure.Services.Pooling;
using UnityEngine;

namespace Codebase.Core.PoolTesting
{
    public class TestPoolableObject : MonoBehaviour, IGetPooledObjectHandler, IReleasePooledObjectHandler, IPoolableObject<TestPoolableObject>
    {
        private ObjectPool<TestPoolableObject> _pool;

        public void OnGet()
        {
            Debug.Log("On get");
            Invoke(nameof(Dispose), 2);
        }

        public void OnRelease()
        {
            Debug.Log("On release");
        }

        public void Construct(ObjectPool<TestPoolableObject> pool)
        {
            _pool = pool;
        }

        public void Dispose()
        {
            _pool?.Release(this);
        }
    }
}
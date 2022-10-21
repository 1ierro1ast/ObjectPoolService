using UnityEngine;

namespace Codebase.Infrastructure.Services.Pooling
{
    public interface IObjectPool<T> where T : Component
    {
        T Create(Vector3 position = default, Quaternion rotation = default, Transform parent = null);

        void Dispose(T disposingObject);
    }
}

using UnityEngine;

namespace Codebase.Infrastructure.Services.Pooling
{
    public interface IObjectsPool<T> : IService where T : MonoBehaviour
    {
        public T Get(Vector3 position = default, Quaternion rotation = default, Transform parent = null);

        public void Release(T releasingObject);
    }
}
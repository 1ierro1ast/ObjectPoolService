
using UnityEngine;

namespace Codebase.Infrastructure.Services.Pooling
{
    public interface IPoolableObject<T> where T : MonoBehaviour
    {
        void Construct(ObjectPool<T> pool);

        void Dispose();
    }
}
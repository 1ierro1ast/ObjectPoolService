using UnityEngine;

namespace Codebase.Infrastructure.Services.Pooling
{
    public class ObjectPoolFactory : IObjectPoolFactory
    {
        public ObjectPool<T> GetPool<T>() where T : Component
        {
            throw new System.NotImplementedException();
        }
    }
}

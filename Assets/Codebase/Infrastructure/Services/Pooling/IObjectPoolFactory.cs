using UnityEngine;

namespace Codebase.Infrastructure.Services.Pooling
{
    public interface IObjectPoolFactory : IService
    {
        public ObjectPool<T> GetPool<T>() where T : Component;
    }
}

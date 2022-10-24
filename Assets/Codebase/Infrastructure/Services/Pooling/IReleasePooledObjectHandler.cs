namespace Codebase.Infrastructure.Services.Pooling
{
    public interface IReleasePooledObjectHandler
    {
        public void OnRelease();
    }
}
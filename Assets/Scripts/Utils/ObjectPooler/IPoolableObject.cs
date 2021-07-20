public interface IPoolableObject
{
    void OnSpawnedFromPool();
    void SetIsFromPool(bool value);
}

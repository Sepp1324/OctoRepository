namespace OctoAwesome.Pooling
{
    public interface IPool
    {
        void Push(IPoolElement obj);
    }

    public interface IPool<T> : IPool where T : IPoolElement
    {
        T Get();

        void Push(T obj);
    }
}
namespace OctoAwesome.Pooling
{
    public interface IPool
    {
        void Push(IPoolElement obj);
    }
<<<<<<< HEAD
    
=======
>>>>>>> feature/performance
    public interface IPool<T> : IPool where T : IPoolElement, new() 
    {
        T Get();

        void Push(T obj);
    }
}

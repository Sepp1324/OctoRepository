namespace OctoAwesome.Components
{
    public interface IHoldComponent<T>
    {
        void Add(T value);
        void Remove(T value);
    }
}
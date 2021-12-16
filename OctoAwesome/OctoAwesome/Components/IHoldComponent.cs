namespace OctoAwesome.Components
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHoldComponent<T>
    {
        void Add(T value);
        void Remove(T value);
    }
}
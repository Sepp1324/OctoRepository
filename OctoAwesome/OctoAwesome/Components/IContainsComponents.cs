namespace OctoAwesome.Components
{
    /// <summary>
    /// </summary>
    public interface IContainsComponents
    {
        bool ContainsComponent<T>();

        T GetComponent<T>();
    }
}
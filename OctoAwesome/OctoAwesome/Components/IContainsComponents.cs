namespace OctoAwesome.Components
{
    public interface IContainsComponents
    {
        bool ContainsComponent<T>();

        T GetComponent<T>();
    }
}
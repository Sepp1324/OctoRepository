namespace OctoAwesome
{
    /// <summary>
    ///     Interface for all Mod Plugin Extensions.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        ///     Gets the Extension Name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the Extension Description.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Register the Components in the ExtensionsLoader
        /// </summary>
        /// <param name="extensionLoader">ExtensionsLoader</param>
        /// <param name="typeContainer">TypeContainer <see cref="ITypeContainer"/></param>
        void Register(IExtensionLoader extensionLoader, ITypeContainer typeContainer);

        /// <summary>
        /// <see cref="ITypeContainer"/>
        /// </summary>
        /// <param name="typeContainer"></param>
        void Register(ITypeContainer typeContainer);
    }
}
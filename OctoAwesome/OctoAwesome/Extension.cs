using engenious;
using OctoAwesome.EntityComponents;

namespace OctoAwesome
{
    //TODO: Outsource

    /// <summary>
    /// Extension for the Main-Game
    /// </summary>
    public sealed class Extension : IExtension
    {
        /// <summary>
        /// Description of the Extension
        /// </summary>
        public string Description => "OctoAwesome";

        /// <summary>
        /// Name of the Extension
        /// </summary>
        public string Name => "OctoAwesome";

<<<<<<< HEAD
        /// <summary>
        /// Register the Extension
        /// </summary>
        /// <param name="extensionLoader"></param>
        /// <param name="typeContainer"></param>
=======
>>>>>>> feature/performance
        public void Register(IExtensionLoader extensionLoader, ITypeContainer typeContainer)
        {
            extensionLoader.RegisterEntityExtender<Player>((p) =>
            {
                p.Components.AddComponent(new ControllableComponent());
                p.Components.AddComponent(new HeadComponent() { Offset = new Vector3(0, 0, 3.2f) });
                p.Components.AddComponent(new InventoryComponent());
                p.Components.AddComponent(new ToolBarComponent());
            });
        }

        public void Register(ITypeContainer typeContainer) { }
    }
}

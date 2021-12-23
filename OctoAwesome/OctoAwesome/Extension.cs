using OctoAwesome.EntityComponents;

namespace OctoAwesome
{
    //TODO:Eventuell auslagern

    /// <summary>
    /// </summary>
    public sealed class Extension : IExtension
    {
        /// <summary>
        /// </summary>
        public string Description => "OctoAwesome";

        /// <summary>
        /// </summary>
        public string Name => "OctoAwesome";

        /// <summary>
        /// </summary>
        /// <param name="extensionLoader"></param>
        /// <param name="typeContainer"></param>
        public void Register(IExtensionLoader extensionLoader, ITypeContainer typeContainer)
        {
            extensionLoader.RegisterEntityExtender<Player>(p =>
            {
                p.Components.AddComponent(new ControllableComponent());
                p.Components.AddComponent(new HeadComponent { Offset = new(0, 0, 3.2f) });
                p.Components.AddComponent(new InventoryComponent());
                p.Components.AddComponent(new ToolBarComponent());
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="typeContainer"></param>
        public void Register(ITypeContainer typeContainer)
        {
        }
    }
}
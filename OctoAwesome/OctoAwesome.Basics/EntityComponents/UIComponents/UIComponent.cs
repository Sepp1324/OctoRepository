using engenious.UI;
using OctoAwesome.Components;
using OctoAwesome.UI.Components;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    public abstract class UIComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        public UIComponent()
        {
            ScreenComponent = TypeContainer.Get<BaseScreenComponent>();
            AssetComponent = TypeContainer.Get<AssetComponent>();
        }

        protected BaseScreenComponent ScreenComponent { get; }
        public AssetComponent AssetComponent { get; }
    }
}
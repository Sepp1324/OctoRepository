using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Controls
{
    internal class HealthBarControl : ProgressBar
    {
        public HealthBarControl(BaseScreenComponent manager, string style = "") : base(manager, style) => Background = Skin.Current.HorizontalScrollBackgroundBrush;
    }
}

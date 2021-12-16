using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.UI.Controls
{
    public class HealthBarControl : ProgressBar
    {
        public HealthBarControl(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            Background = Skin.Current.HorizontalScrollBackgroundBrush;
        }
    }
}
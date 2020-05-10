using MonoGameUi;

namespace OctoAwesome.Client.Controls
{
    internal class HealthBarControl : ProgressBar
    {
        public HealthBarControl(IScreenManager manager, string style = "") : base(manager, style)
        {
            Background = Skin.Current.HorizontalScrollBackgroundBrush;
        }
    }
}

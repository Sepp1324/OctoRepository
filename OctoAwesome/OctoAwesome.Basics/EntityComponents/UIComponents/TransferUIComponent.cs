using System;
using engenious.UI;
using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    public class TransferUIComponent : UIComponent
    {
        private readonly InventoryComponent _chestInventory;

        private readonly TransferScreen _transferScreen;

        public TransferUIComponent(InventoryComponent chestInventory)
        {
            _chestInventory = chestInventory;
            _transferScreen = new(ScreenComponent, AssetComponent, chestInventory, new());
            _transferScreen.Closed += TransferScreen_Closed;
        }

        public event EventHandler<NavigationEventArgs> Closed;

        private void TransferScreen_Closed(object sender, NavigationEventArgs e) => Closed?.Invoke(sender, e);

        public void Show(Player p)
        {
            var playerInventory = p.Components.GetComponent<InventoryComponent>();

            if (playerInventory is null)
                return;

            _transferScreen.Rebuild(_chestInventory, playerInventory);

            if (ScreenComponent.ActiveScreen != _transferScreen)
                ScreenComponent.NavigateToScreen(_transferScreen);
        }
    }
}
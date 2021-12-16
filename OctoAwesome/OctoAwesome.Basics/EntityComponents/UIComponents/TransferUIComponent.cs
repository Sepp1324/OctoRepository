using System;
using engenious.UI;
using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    public class TransferUIComponent : UIComponent
    {
        private readonly InventoryComponent chestInventory;

        private readonly TransferScreen transferScreen;

        public TransferUIComponent(InventoryComponent chestInventory)
        {
            this.chestInventory = chestInventory;
            transferScreen =
                new TransferScreen(ScreenComponent, AssetComponent, chestInventory, new InventoryComponent());
            transferScreen.Closed += TransferScreen_Closed;
        }

        public event EventHandler<NavigationEventArgs> Closed;

        private void TransferScreen_Closed(object sender, NavigationEventArgs e)
        {
            Closed?.Invoke(sender, e);
        }

        public void Show(Player p)
        {
            var playerInventory = p.Components.GetComponent<InventoryComponent>();

            if (playerInventory is null)
                return;

            transferScreen.Rebuild(chestInventory, playerInventory);

            if (ScreenComponent.ActiveScreen != transferScreen)
                ScreenComponent.NavigateToScreen(transferScreen);
        }
    }
}
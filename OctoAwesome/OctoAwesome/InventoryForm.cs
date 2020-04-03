using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OctoAwesome
{
    public partial class InventoryForm : Form
    {
        private IHaveInventory left;
        private IHaveInventory right;

        public InventoryForm()
        {
            InitializeComponent();
        }

        public void Init(IHaveInventory left, IHaveInventory right)
        {
            this.left = left;
            this.right = right;

            listViewPlayer.Items.Clear();

            foreach(var inventoryItem in left.InventoryItems)
            {
                ListViewItem item = listViewPlayer.Items.Add(inventoryItem.Name);
                item.Tag = inventoryItem;
            }

            listViewBox.Items.Clear();
            foreach (var inventoryItem in right.InventoryItems)
            {
                ListViewItem item = listViewBox.Items.Add(inventoryItem.Name);
                item.Tag = inventoryItem;
            }
        }

        private void listViewPlayer_DoubleClick(object sender, EventArgs e)
        {
            if (listViewPlayer.SelectedItems.Count > 0)
            {
                ListViewItem item = listViewPlayer.SelectedItems[0];
                InventoryItem inventoryItem = item.Tag as InventoryItem;

                left.InventoryItems.Remove(inventoryItem);
                right.InventoryItems.Add(inventoryItem);

                listViewPlayer.Items.Remove(item);

                ListViewItem item2 = listViewBox.Items.Add(inventoryItem.Name);
                item2.Tag = inventoryItem;
            }
        }

        private void listViewBox_DoubleClick(object sender, EventArgs e)
        {
            if (listViewBox.SelectedItems.Count > 0)
            {
                ListViewItem item = listViewBox.SelectedItems[0];
                InventoryItem inventoryItem = item.Tag as InventoryItem;

                right.InventoryItems.Remove(inventoryItem);
                left.InventoryItems.Add(inventoryItem);

                listViewBox.Items.Remove(item);

                ListViewItem item2 = listViewPlayer.Items.Add(inventoryItem.Name);
                item2.Tag = inventoryItem;
            }
        }

        private void InventoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

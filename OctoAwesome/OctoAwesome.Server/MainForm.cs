using OctoAwesome.Runtime;
using System;
using System.Linq;
using System.Windows.Forms;

namespace OctoAwesome.Server
{
    public partial class MainForm : Form
    {
        private World world;

        public MainForm()
        {
            InitializeComponent();

            Runtime.Server.Instance.OnRegister += Instance_OnRegister;
            Runtime.Server.Instance.OnDeregister += Instance_OnDeregister;

            listBox1.DisplayMember = "Playername";

            world = new World();

            Runtime.Server.Instance.Open();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Runtime.Server.Instance.Close();
            base.OnFormClosed(e);
        }


        private void Instance_OnDeregister(Client client)
        {
            listBox1.Items.Remove(client);
        }

        private void Instance_OnRegister(Client client)
        {
            listBox1.Items.Add(client);
        }
    }
}

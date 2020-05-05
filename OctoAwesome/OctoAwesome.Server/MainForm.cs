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
            Runtime.Server.Instance.Open();
            timer1.Enabled = true;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            world = new World(); 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Runtime.Server.Instance.Clients.ToArray());
        }
    }
}

using OctoAwesome.Runtime;
using System.Windows.Forms;

namespace OctoAwesome.Server
{
    public partial class MainForm : Form
    {
        private World world;

        public MainForm()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, System.EventArgs e)
        {
            world = new World(); //CONTINUE 280
        }
    }
}

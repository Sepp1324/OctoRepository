using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OctoAwesome
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            watch.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            x += (int)(dx * watch.Elapsed.TotalSeconds);
            y += (int)(dy * watch.Elapsed.TotalSeconds);

            if (x < 0)
            {
                dx *= -1;
                x = 0;
            }

            if ((x + 100) > renderPanel.ClientRectangle.Width)
            {
                dx *= -1;
                x = renderPanel.ClientRectangle.Width - 100;
            }

            if (y < 0)
            {
                dy *= -1;
                y = 0;
            }

            if ((y + 100) > renderPanel.ClientRectangle.Height)
            {
                dy *= -1;
                y = renderPanel.ClientRectangle.Height - 100;
            }
            watch.Restart();
            renderPanel.Invalidate();
        }


        Stopwatch watch = new Stopwatch();

        private int x = 0;
        private int y = 0;

        private int dx = 100;
        private int dy = 80;

        private void renderPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.CornflowerBlue);

            using (Brush brush = new SolidBrush(Color.White))
            {
                e.Graphics.FillEllipse(brush, new Rectangle(x, y, 100, 100));
            }
        }

        private void closeMenu_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

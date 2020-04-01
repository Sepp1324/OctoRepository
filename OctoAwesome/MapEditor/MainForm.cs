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
using static OctoAwesome.Model.Map;

namespace MapEditor
{
    public partial class renderPanl : Form
    {
        private Map map;

        public renderPanl()
        {
            InitializeComponent();

            timer.Enabled = true;
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            int cellSize = 20;

            e.Graphics.Clear(Color.CornflowerBlue);

            if (map == null)
                return;

            SolidBrush sandBrush = new SolidBrush(Color.SandyBrown);
            SolidBrush grassBrush = new SolidBrush(Color.DarkGreen);
            SolidBrush waterBrush = new SolidBrush(Color.Blue);

            for (int x = 0; x < map.Cells.GetLength(0); x++)
            {
                for (int y = 0; y < map.Cells.GetLength(1); y++)
                {
                    SolidBrush brush = null;

                    switch (map.Cells[x, y])
                    {
                        case CellType.Grass:
                            brush = grassBrush;
                            break;
                        case CellType.Sand:
                            brush = sandBrush;
                            break;
                        case CellType.Water:
                            brush = waterBrush;
                            break;
                    }

                    if (brush == null)
                        continue;

                    e.Graphics.FillRectangle(brush, new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize));
                }
            }

            using (Pen pen = new Pen(Color.White))
            {

                for (int x = 0; x < map.Cells.GetLength(0); x++)
                {
                    e.Graphics.DrawLine(pen, new Point(x * cellSize, 0), new Point(x * cellSize, map.Cells.GetLength(1) * cellSize));
                }
                for (int y = 0; y < map.Cells.GetLength(1); y++)
                {
                    e.Graphics.DrawLine(pen, new Point(0, y * cellSize), new Point(map.Cells.GetLength(0) * cellSize, y * cellSize));
                }
            }

            sandBrush.Dispose();
            grassBrush.Dispose();
            waterBrush.Dispose();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            panel.Invalidate();
        }

        private void closeMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void x20Menu_Click(object sender, EventArgs e)
        {
            map = new Map(20, 20);
        }

        private void x40Menu_Click(object sender, EventArgs e)
        {
            map = new Map(40, 40);
        }
    }
}

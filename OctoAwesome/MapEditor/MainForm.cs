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

namespace MapEditor
{
    public partial class MainForm : Form
    {
        private Map map;

        private bool mouseActive = false;
        private bool mouseDraw = false;

        private Point mousePosition = new Point();
        private int cellSize = 20;

        private CellType drawMode = CellType.Grass;

        public MainForm()
        {
            InitializeComponent();

            timer.Enabled = true;
        }

        private void closeMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void renderPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.CornflowerBlue);

            if (map == null)
                return;

            SolidBrush sandBrush = new SolidBrush(Color.SandyBrown);
            SolidBrush grassBrush = new SolidBrush(Color.DarkGreen);
            SolidBrush waterBrush = new SolidBrush(Color.Blue);
            SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(100, Color.White));

            for (int x = 0; x < map.Cells.GetLength(0); x++)
            {
                for (int y = 0; y < map.Cells.GetLength(1); y++)
                {
                    SolidBrush brush = null;

                    switch (map.Cells[x, y])
                    {
                        case CellType.Grass: brush = grassBrush; break;
                        case CellType.Sand: brush = sandBrush; break;
                        case CellType.Water: brush = waterBrush; break;
                    }
                    if (brush == null)
                        continue;

                    e.Graphics.FillRectangle(brush, new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize));
                }
            }

            if(mouseActive)
            {
                e.Graphics.FillRectangle(selectionBrush, new Rectangle(mousePosition.X * cellSize, mousePosition.Y * cellSize, cellSize, cellSize));
            }

            using (Pen pen = new Pen(Color.FromArgb(100, Color.White)))
            {
                for (int x = 0; x < map.Cells.GetLength(0) + 1; x++)
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
            renderPanel.Invalidate();
            cellPositionLabel.Text = (mouseActive ? mousePosition.X + "/" + mousePosition.Y : string.Empty);

            saveMenu.Enabled = map != null;
        }

        private void smallMapMenu_Click(object sender, EventArgs e)
        {
            map = new Map(20, 20);
        }

        private void mediumMapMenu_Click(object sender, EventArgs e)
        {
            map = new Map(40, 40);
        }

        private void renderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            mousePosition = new Point((int)(e.X / cellSize), (int)(e.Y / cellSize));

            if (map == null || !mouseDraw || !mouseActive)
                return;

            if (mousePosition.X < 0 || mousePosition.X >= map.Cells.GetLength(0) || mousePosition.Y < 0 || mousePosition.Y >= map.Cells.GetLength(1))
                return;

            map.Cells[mousePosition.X, mousePosition.Y] = drawMode;
        }

        private void renderPanel_MouseEnter(object sender, EventArgs e)
        {
            mouseActive = true;
        }

        private void renderPanel_MouseLeave(object sender, EventArgs e)
        {
            mouseActive = false;
        }

        private void sandButton_Click(object sender, EventArgs e)
        {
            drawMode = CellType.Sand;

            grassButton.Checked = false;
            sandButton.Checked = true;
            waterButton.Checked = false;
        }

        private void grassButton_Click(object sender, EventArgs e)
        {
            drawMode = CellType.Grass;

            grassButton.Checked = true;
            sandButton.Checked = false;
            waterButton.Checked = false;
        }

        private void waterButton_Click(object sender, EventArgs e)
        {
            drawMode = CellType.Water;

            grassButton.Checked = false;
            sandButton.Checked = false;
            waterButton.Checked = true;
        }

        private void renderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                mouseDraw = true;
            }
        }

        private void renderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                mouseDraw = false;
            }
        }

        private void saveMenu_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if( saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Map.Save(saveFileDialog.FileName, map);
            }
        }

        private void loadMenu_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                map = Map.Load(openFileDialog.FileName);
            }
        }
    }
}

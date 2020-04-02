using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using OctoAwesome.Model;
using OctoAwesome.Rendering;

namespace OctoAwesome
{
    internal partial class RenderControl : UserControl
    {
        private const int SPRITE_WIDTH = 57;
        private const int SPRITE_HEIGHT = 57;

        private Stopwatch watch = new Stopwatch();

        private readonly Game game;

        private readonly Image grass;
       /* private readonly Image sand_center;
        private readonly Image sand_left;
        private readonly Image sand_right;
        private readonly Image sand_upper;
        private readonly Image sand_lower;
        private readonly Image sand_upperLeft_concarve;
        private readonly Image sand_upperRight_concarve;
        private readonly Image sand_lowerLeft_concarve;
        private readonly Image sand_lowerRight_concarve;
        private readonly Image sand_upperLeft_convex;
        private readonly Image sand_upperRight_convex;
        private readonly Image sand_lowerLeft_convex;
        private readonly Image sand_lowerRight_convex;*/
        private readonly Image water;

        private readonly Image sprite;

        private readonly CellTypeRenderer sandRenderer;

        public RenderControl(Game game)
        {
            InitializeComponent();

            this.game = game;

            game.Camera.SetRenderSize(new Vector2(ClientSize.Width, ClientSize.Height));

            grass = Image.FromFile("Assets/grass_center.png");
            water = Image.FromFile("Assets/water_center.png");
            sprite = Image.FromFile("Assets/Sprite.png");

            sandRenderer = new CellTypeRenderer("sand");

            /*sand_center = Image.FromFile("Assets/sand_center.png");
            sand_left = Image.FromFile("Assets/sand_left.png");
            sand_right = Image.FromFile("Assets/sand_right.png");
            sand_upper = Image.FromFile("Assets/sand_upper.png");
            sand_lower = Image.FromFile("Assets/sand_lower.png");
            sand_upperLeft_concarve = Image.FromFile("Assets/sand_upperLeft_concave.png");
            sand_upperRight_concarve = Image.FromFile("Assets/sand_upperRight_concave.png");
            sand_lowerLeft_concarve = Image.FromFile("Assets/sand_lowerLeft_concave.png");
            sand_lowerRight_concarve = Image.FromFile("Assets/sand_lowerRight_concave.png");
            sand_upperLeft_convex = Image.FromFile("Assets/sand_upperLeft_convex.png");
            sand_upperRight_convex = Image.FromFile("Assets/sand_upperRight_convex.png");
            sand_lowerLeft_convex = Image.FromFile("Assets/sand_lowerLeft_convex.png");
            sand_lowerRight_convex = Image.FromFile("Assets/sand_lowerRight_convex.png");*/
          
            watch.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            if (game != null)
            {
                game.Camera.SetRenderSize(new Vector2(ClientSize.Width, ClientSize.Height));
            }
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(63, 25, 0));

            if (game == null)
                return;

            int cellX1 = Math.Max(0, (int)(game.Camera.ViewPort.X / game.Camera.SCALE));
            int cellY1 = Math.Max(0, (int)(game.Camera.ViewPort.Y / game.Camera.SCALE));

            int cellCountX = (int)(ClientSize.Width / game.Camera.SCALE) + 2;
            int cellCountY = (int)(ClientSize.Height / game.Camera.SCALE) + 2;

            int cellX2 = Math.Min(cellX1 + cellCountX, (int)(game.PlaygroundSize.X));
            int cellY2 = Math.Min(cellY1 + cellCountY, (int)(game.PlaygroundSize.Y));

            for (int x = cellX1; x < cellX2; x++)
            {
                for (int y = cellY1; y < cellY2; y++)
                {
                    switch (game.Map.GetCell(x, y))
                    {
                        case CellType.Grass:
                            e.Graphics.DrawImage(grass, new Rectangle((int)(x * game.Camera.SCALE - game.Camera.ViewPort.X), (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y), (int)game.Camera.SCALE, (int)game.Camera.SCALE));
                            break;

                        case CellType.Sand:
                            //DrawSand(e.Graphics, x, y);
                            sandRenderer.Draw(e.Graphics, game, x, y);
                            break;

                        case CellType.Water:
                            e.Graphics.DrawImage(water, new Rectangle((int)(x * game.Camera.SCALE - game.Camera.ViewPort.X), (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y), (int)game.Camera.SCALE, (int)game.Camera.SCALE));
                            break;
                    }
                }
            }

            int frame = (int)((watch.ElapsedMilliseconds / 250) % 4);

            int offsetx = 0;

            if (game.Player.State == PlayerState.WALK)
            {

                switch (frame)
                {
                    case 0: offsetx = 0; break;
                    case 1: offsetx = SPRITE_WIDTH; break;
                    case 2: offsetx = 2 * SPRITE_WIDTH; break;
                    case 3: offsetx = SPRITE_WIDTH; break;
                }
            }
            else
            {
                offsetx = SPRITE_WIDTH;
            }
            //Umrechnung in Grad
            float direction = (game.Player.Angle * 360f) / (float)(2 * Math.PI);

            //in positiven BEreich
            direction += 180;

            //offset
            direction += 45;

            int sector = (int)(direction / 90);

            int offsety = 0;

            switch (sector)
            {
                case 1: offsety = 3 * SPRITE_HEIGHT; break;
                case 2: offsety = 2 * SPRITE_HEIGHT; break;
                case 3: offsety = 0 * SPRITE_HEIGHT; break;
                case 4: offsety = 1 * SPRITE_HEIGHT; break;
            }

            Point spriteCenter = new Point(27, 48);

            e.Graphics.DrawImage(sprite, new RectangleF((game.Player.Position.X * game.Camera.SCALE) - game.Camera.ViewPort.X - spriteCenter.X, (game.Player.Position.Y * game.Camera.SCALE) - game.Camera.ViewPort.Y - spriteCenter.Y, SPRITE_WIDTH, SPRITE_HEIGHT), new RectangleF(offsetx, offsety, SPRITE_WIDTH, SPRITE_HEIGHT), GraphicsUnit.Pixel);
        }

        /*private void DrawSand(Graphics g, int x, int y)
        {
            //g.DrawImage(sand_center, new Rectangle((int)(x * game.Camera.SCALE - game.Camera.ViewPort.X), (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y), (int)game.Camera.SCALE, (int)game.Camera.SCALE));
             DrawTexture(g, x, y, sand_center);

            bool left = x > 0 && game.Map.GetCell(x - 1, y) != CellType.Sand;
            bool top = y > 0 && game.Map.GetCell(x, y - 1) != CellType.Sand;
            bool right = (x + 1) < game.Map.Columns && game.Map.GetCell(x + 1, y) != CellType.Sand;
            bool bottom = (y + 1) < game.Map.Rows && game.Map.GetCell(x, y + 1) != CellType.Sand;

            bool upperLeft = x > 0 && y > 0 && game.Map.GetCell(x - 1, y - 1) != CellType.Sand;
            bool upperRight = (x + 1) < game.Map.Columns && y > 0 && game.Map.GetCell(x + 1, y - 1) != CellType.Sand;
            bool lowerLeft = x > 0 && (y + 1) < game.Map.Rows && game.Map.GetCell(x - 1, y + 1) != CellType.Sand;
            bool lowerRight = (x + 1) < game.Map.Columns && (y + 1) < game.Map.Rows && game.Map.GetCell(x + 1, y + 1) != CellType.Sand;

            //Gerade Kanten
            if (left) DrawTexture(g, x, y, sand_left);
            if (right) DrawTexture(g, x, y, sand_right);
            if (top) DrawTexture(g, x, y, sand_upper);
            if (bottom) DrawTexture(g, x, y, sand_lower);

            //Konvexe Ecken
            if (left && top) DrawTexture(g, x, y, sand_upperLeft_convex);
            if (left && bottom) DrawTexture(g, x, y, sand_lowerLeft_convex);
            if (right && top) DrawTexture(g, x, y, sand_upperRight_convex);
            if (right && bottom) DrawTexture(g, x, y, sand_lowerRight_convex);

            //Konkave Ecken
            if (upperLeft && !left && !top) DrawTexture(g, x, y, sand_upperLeft_concarve);
            if (upperRight && !right && !top) DrawTexture(g, x, y, sand_upperRight_concarve);
            if (lowerLeft && !left && !bottom) DrawTexture(g, x, y, sand_lowerLeft_concarve);
            if (lowerRight && !right && !bottom) DrawTexture(g, x, y, sand_lowerRight_concarve);
        }*/


    }
}


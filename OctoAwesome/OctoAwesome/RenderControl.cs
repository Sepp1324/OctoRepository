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

namespace OctoAwesome
{
    internal partial class RenderControl : UserControl
    {
        private const int SPRITE_WIDTH = 57;
        private const int SPRITE_HEIGHT = 57;

        private Stopwatch watch = new Stopwatch();

        //internal Game Game { get; private set; }
        private readonly Game game;

        private readonly Image grass;
        private readonly Image sprite;

        public RenderControl(Game game)
        {
            InitializeComponent();

            this.game = game;

            grass = Image.FromFile("Assets/grass.png");
            sprite = Image.FromFile("Assets/Sprite.png");

            watch.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            if (game != null)
            {
               // Game.PlaygroundSize = new Point(ClientSize.Width, ClientSize.Height);
            }
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.CornflowerBlue);

            int offsetX = (int)game.Camera.Center.X - (this.ClientSize.Width / 2);
            int offsetY = (int)game.Camera.Center.Y - (this.ClientSize.Height / 2);

            int cellX1 = Math.Max(0, (int)(offsetX / 100));
            int cellY1 = Math.Max(0, (int)(offsetY / 100));

            int cellCountX = (ClientSize.Width / grass.Width) + 2;
            int cellCountY = (ClientSize.Height / grass.Height) + 2;

            int cellX2 = Math.Min(cellX1 + cellCountX, (int)(game.PlaygroundSize.X / grass.Width));
            int cellY2 = Math.Min(cellY1 + cellCountY, (int)(game.PlaygroundSize.Y / grass.Height));

            for (int x = cellX1; x < cellX2; x++)
            {
                for (int y = cellY1; y < cellY2; y++)
                {
                    e.Graphics.DrawImage(grass, new Point(x * grass.Width - offsetX, y * grass.Height - offsetY));
                }
            }

            if (game == null)
                return;

            using (Brush brush = new SolidBrush(Color.White))
            {
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

                e.Graphics.DrawImage(sprite, new RectangleF(game.Player.Position.X - offsetX, game.Player.Position.Y - offsetY, SPRITE_WIDTH, SPRITE_HEIGHT), new RectangleF(offsetx, offsety, SPRITE_WIDTH, SPRITE_HEIGHT), GraphicsUnit.Pixel);
            }
        }
    }
}

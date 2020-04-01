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
    public partial class RenderControl : UserControl
    {
        private const int SPRITE_WIDTH = 57;
        private const int SPRITE_HEIGHT = 57;

        private Stopwatch watch = new Stopwatch();

        internal Game Game { get; set; }

        private Image grass;
        private Image sprite;

        public RenderControl()
        {
            InitializeComponent();

            grass = Image.FromFile("Assets/grass.png");
            sprite = Image.FromFile("Assets/Sprite.png");

            watch.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            if (Game != null)
            {
                Game.PlaygroundSize = new Point(ClientSize.Width, ClientSize.Height);
            }
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.CornflowerBlue);

            for (int x = 0; x < ClientRectangle.Width; x += grass.Width)
            {
                for (int y = 0; y < ClientRectangle.Height; y += grass.Height)
                {
                    e.Graphics.DrawImage(grass, new Point(x, y));
                }
            }

            if (Game == null)
                return;

            using (Brush brush = new SolidBrush(Color.White))
            {
                int frame = (int)((watch.ElapsedMilliseconds / 250) % 4);

                int offsetx = 0;

                if (Game.Player.State == PlayerState.WALK)
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
                float direction = (Game.Player.Angle * 360f) / (float)(2 * Math.PI);

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

                e.Graphics.DrawImage(sprite, new RectangleF(Game.Player.Position.X, Game.Player.Position.Y, SPRITE_WIDTH, SPRITE_HEIGHT), new RectangleF(offsetx, offsety, SPRITE_WIDTH, SPRITE_HEIGHT), GraphicsUnit.Pixel);
            }
        }
    }
}

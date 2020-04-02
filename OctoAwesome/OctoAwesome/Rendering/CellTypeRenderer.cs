using OctoAwesome.Components;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Rendering
{
    internal class CellTypeRenderer
    {
        private readonly Image center;
        private readonly Image left;
        private readonly Image right;
        private readonly Image upper;
        private readonly Image lower;
        private readonly Image upperLeft_concarve;
        private readonly Image upperRight_concarve;
        private readonly Image lowerLeft_concarve;
        private readonly Image lowerRight_concarve;
        private readonly Image upperLeft_convex;
        private readonly Image upperRight_convex;
        private readonly Image lowerLeft_convex;
        private readonly Image lowerRight_convex;

        public CellTypeRenderer(string name)
        {
            center = Image.FromFile(string.Format("Assets/{0}_center.png", name));
            left = Image.FromFile(string.Format("Assets/{0}_left.png", name));
            right = Image.FromFile(string.Format("Assets/{0}_right.png", name));
            upper = Image.FromFile(string.Format("Assets/{0}_upper.png", name));
            lower = Image.FromFile(string.Format("Assets/{0}_lower.png", name));
            upperLeft_concarve = Image.FromFile(string.Format("Assets/{0}_upperLeft_concave.png", name));
            upperRight_concarve = Image.FromFile(string.Format("Assets/{0}_upperRight_concave.png", name));
            lowerLeft_concarve = Image.FromFile(string.Format("Assets/{0}_lowerLeft_concave.png", name));
            lowerRight_concarve = Image.FromFile(string.Format("Assets/{0}_lowerRight_concave.png", name));
            upperLeft_convex = Image.FromFile(string.Format("Assets/{0}_upperLeft_convex.png", name));
            upperRight_convex = Image.FromFile(string.Format("Assets/{0}_upperRight_convex.png", name));
            lowerLeft_convex = Image.FromFile(string.Format("Assets/{0}_lowerLeft_convex.png", name));
            lowerRight_convex = Image.FromFile(string.Format("Assets/{0}_lowerRight_convex.png", name));
        }

        public void Draw(Graphics g, Game game,  int x, int y)
        {
            g.DrawImage(center, new Rectangle((int)(x * game.Camera.SCALE - game.Camera.ViewPort.X), (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y), (int)game.Camera.SCALE, (int)game.Camera.SCALE));
            //DrawTexture(g, x, y, sand_center);

            bool emptyLeft = x > 0 && game.Map.GetCell(x - 1, y) != CellType.Sand;
            bool emptyTop = y > 0 && game.Map.GetCell(x, y - 1) != CellType.Sand;
            bool emptyRight = (x + 1) < game.Map.Columns && game.Map.GetCell(x + 1, y) != CellType.Sand;
            bool emptyBottom = (y + 1) < game.Map.Rows && game.Map.GetCell(x, y + 1) != CellType.Sand;

            bool upperLeft = x > 0 && y > 0 && game.Map.GetCell(x - 1, y - 1) != CellType.Sand;
            bool upperRight = (x + 1) < game.Map.Columns && y > 0 && game.Map.GetCell(x + 1, y - 1) != CellType.Sand;
            bool lowerLeft = x > 0 && (y + 1) < game.Map.Rows && game.Map.GetCell(x - 1, y + 1) != CellType.Sand;
            bool lowerRight = (x + 1) < game.Map.Columns && (y + 1) < game.Map.Rows && game.Map.GetCell(x + 1, y + 1) != CellType.Sand;

            //Gerade Kanten
            if (emptyLeft) DrawTexture(g, game.Camera, x, y, left);
            if (emptyRight) DrawTexture(g, game.Camera, x, y, right);
            if (emptyTop) DrawTexture(g, game.Camera, x, y, upper);
            if (emptyBottom) DrawTexture(g, game.Camera, x, y, lower);

            //Konvexe Ecken
            if (emptyLeft && emptyTop) DrawTexture(g, game.Camera, x, y, upperLeft_convex);
            if (emptyLeft && emptyBottom) DrawTexture(g, game.Camera, x, y, lowerLeft_convex);
            if (emptyRight && emptyTop) DrawTexture(g, game.Camera, x, y, upperRight_convex);
            if (emptyRight && emptyBottom) DrawTexture(g, game.Camera, x, y, lowerRight_convex);

            //Konkave Ecken
            if (upperLeft && !emptyLeft && !emptyTop) DrawTexture(g, game.Camera, x, y, upperLeft_concarve);
            if (upperRight && !emptyRight && !emptyTop) DrawTexture(g, game.Camera, x, y, upperRight_concarve);
            if (lowerLeft && !emptyLeft && !emptyBottom) DrawTexture(g, game.Camera, x, y, lowerLeft_concarve);
            if (lowerRight && !emptyRight && !emptyBottom) DrawTexture(g, game.Camera, x, y, lowerRight_concarve);
        }

        private static void DrawTexture(Graphics g, Camera camera,  int x, int y, Image image)
        {
            g.DrawImage(image, new Rectangle((int)(x * camera.SCALE - camera.ViewPort.X), (int)(y * camera.SCALE - camera.ViewPort.Y), (int)camera.SCALE, (int)camera.SCALE));

        }
    }
}

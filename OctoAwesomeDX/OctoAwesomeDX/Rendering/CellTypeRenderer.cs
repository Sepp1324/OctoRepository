using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Components;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Rendering
{
    internal class CellTypeRenderer
    {
        private readonly Texture2D center;
        private readonly Texture2D left;
        private readonly Texture2D right;
        private readonly Texture2D upper;
        private readonly Texture2D lower;
        private readonly Texture2D upperLeft_concarve;
        private readonly Texture2D upperRight_concarve;
        private readonly Texture2D lowerLeft_concarve;
        private readonly Texture2D lowerRight_concarve;
        private readonly Texture2D upperLeft_convex;
        private readonly Texture2D upperRight_convex;
        private readonly Texture2D lowerLeft_convex;
        private readonly Texture2D lowerRight_convex;

        public CellTypeRenderer(ContentManager content,  string name)
        {
            center = content.Load<Texture2D>(string.Format("Textures/{0}_center", name));//Image.FromFile(string.Format("Assets/{0}_center.png", name));
            left = content.Load<Texture2D>(string.Format("Textures/{0}_left", name)); //Image.FromFile(string.Format("Assets/{0}_left.png", name));
            right = content.Load<Texture2D>(string.Format("Textures/{0}_right", name)); //Image.FromFile(string.Format("Assets/{0}_right.png", name));
            upper = content.Load<Texture2D>(string.Format("Textures/{0}_upper", name)); //Image.FromFile(string.Format("Assets/{0}_upper.png", name));
            lower = content.Load<Texture2D>(string.Format("Textures/{0}_lower", name)); //Image.FromFile(string.Format("Assets/{0}_lower.png", name));
            upperLeft_concarve = content.Load<Texture2D>(string.Format("Textures/{0}_upperLeft_concave", name)); //Image.FromFile(string.Format("Assets/{0}_upperLeft_concave.png", name));
            upperRight_concarve = content.Load<Texture2D>(string.Format("Textures/{0}_upperRight_concave", name)); //Image.FromFile(string.Format("Assets/{0}_upperRight_concave.png", name));
            lowerLeft_concarve = content.Load<Texture2D>(string.Format("Textures/{0}_lowerLeft_concave", name)); //Image.FromFile(string.Format("Assets/{0}_lowerLeft_concave.png", name));
            lowerRight_concarve = content.Load<Texture2D>(string.Format("Textures/{0}_lowerRight_concave", name)); //Image.FromFile(string.Format("Assets/{0}_lowerRight_concave.png", name));
            upperLeft_convex = content.Load<Texture2D>(string.Format("Textures/{0}_upperLeft_convex", name)); //Image.FromFile(string.Format("Assets/{0}_upperLeft_convex.png", name));
            upperRight_convex = content.Load<Texture2D>(string.Format("Textures/{0}_upperRight_convex", name)); //Image.FromFile(string.Format("Assets/{0}_upperRight_convex.png", name));
            lowerLeft_convex = content.Load<Texture2D>(string.Format("Textures/{0}_lowerLeft_convex", name)); //Image.FromFile(string.Format("Assets/{0}_lowerLeft_convex.png", name));
            lowerRight_convex = content.Load<Texture2D>(string.Format("Textures/{0}_lowerRight_convex", name)); //Image.FromFile(string.Format("Assets/{0}_lowerRight_convex.png", name));
        }

        public void Draw(SpriteBatch g, CameraComponent camera, OctoAwesome.Model.World game, int x, int y)
        {
            CellType centerType = game.Map.GetCell(x, y);

            g.Draw(center, new Rectangle((int)(x * camera.SCALE - camera.ViewPort.X), (int)(y * camera.SCALE - camera.ViewPort.Y), (int)camera.SCALE, (int)camera.SCALE), Color.White);

            bool emptyLeft = x > 0 && game.Map.GetCell(x - 1, y) != centerType;
            bool emptyTop = y > 0 && game.Map.GetCell(x, y - 1) != centerType;
            bool emptyRight = (x + 1) < game.Map.Columns && game.Map.GetCell(x + 1, y) != centerType;
            bool emptyBottom = (y + 1) < game.Map.Rows && game.Map.GetCell(x, y + 1) != centerType;

            bool upperLeft = x > 0 && y > 0 && game.Map.GetCell(x - 1, y - 1) != centerType;
            bool upperRight = (x + 1) < game.Map.Columns && y > 0 && game.Map.GetCell(x + 1, y - 1) != centerType;
            bool lowerLeft = x > 0 && (y + 1) < game.Map.Rows && game.Map.GetCell(x - 1, y + 1) != centerType;
            bool lowerRight = (x + 1) < game.Map.Columns && (y + 1) < game.Map.Rows && game.Map.GetCell(x + 1, y + 1) != centerType;

            //Gerade Kanten
            if (emptyLeft) DrawTexture(g, camera, x, y, left);
            if (emptyRight) DrawTexture(g, camera, x, y, right);
            if (emptyTop) DrawTexture(g, camera, x, y, upper);
            if (emptyBottom) DrawTexture(g, camera, x, y, lower);

            //Konvexe Ecken
            if (emptyLeft && emptyTop) DrawTexture(g, camera, x, y, upperLeft_convex);
            if (emptyLeft && emptyBottom) DrawTexture(g, camera, x, y, lowerLeft_convex);
            if (emptyRight && emptyTop) DrawTexture(g, camera, x, y, upperRight_convex);
            if (emptyRight && emptyBottom) DrawTexture(g, camera, x, y, lowerRight_convex);

            //Konkave Ecken
            if (upperLeft && !emptyLeft && !emptyTop) DrawTexture(g, camera, x, y, upperLeft_concarve);
            if (upperRight && !emptyRight && !emptyTop) DrawTexture(g, camera, x, y, upperRight_concarve);
            if (lowerLeft && !emptyLeft && !emptyBottom) DrawTexture(g, camera, x, y, lowerLeft_concarve);
            if (lowerRight && !emptyRight && !emptyBottom) DrawTexture(g, camera, x, y, lowerRight_concarve);
        }

        private static void DrawTexture(SpriteBatch g, CameraComponent camera, int x, int y, Texture2D image)
        {
            g.Draw(image, new Rectangle((int)(x * camera.SCALE - camera.ViewPort.X), (int)(y * camera.SCALE - camera.ViewPort.Y), (int)camera.SCALE, (int)camera.SCALE), Color.White);

        }
    }
}

namespace OctoAwesome.Basics
{
    public class WoodBlock : Block
    {
        public WoodBlock()
        {
            TopTexture = 0;
            BottomTexture = 0;
            NorthTexture = 1;
            SouthTexture = 1;
            WestTexture = 1;
            EastTexture = 1;
        }

        public void Update()
        {
            switch (Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                    TopTexture = 1;
                    BottomTexture = 1;
                    NorthTexture = 1;
                    SouthTexture = 1;
                    WestTexture = 0;
                    EastTexture = 0;
                    break;

                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    TopTexture = 1;
                    BottomTexture = 1;
                    NorthTexture = 0;
                    SouthTexture = 0;
                    WestTexture = 1;
                    EastTexture = 1;
                    break;

                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                    TopTexture = 0;
                    BottomTexture = 0;
                    NorthTexture = 1;
                    SouthTexture = 1;
                    WestTexture = 1;
                    EastTexture = 1;
                    break;
            }
        }
    }
}

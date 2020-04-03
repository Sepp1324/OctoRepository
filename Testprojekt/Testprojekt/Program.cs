using OctoAwesomeDX;
using System;

namespace Testprojekt
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new OctoGame())
                game.Run();
        }
    }
#endif
}

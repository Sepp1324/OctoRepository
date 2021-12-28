#region Using Statements

using System;
using System.IO;
using OctoAwesome.Logging;

#endregion

namespace OctoAwesome.Client
{
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {
        private static OctoGame _game;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var typeContainer = TypeContainer.Get<ITypeContainer>())
            {
                Startup.Register(typeContainer);
                Startup.ConfigureLogger(ClientType.DesktopClient);
                Network.Startup.Register(typeContainer);

                var logger = (typeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As("OctoAwesome.Client");
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    File.WriteAllText(Path.Combine(".", "logs", $"client-dump-{DateTime.Now:ddMMyy_hhmmss}.txt"), e.ExceptionObject.ToString());

                    logger.Fatal($"Unhandled Exception: {e.ExceptionObject}", e.ExceptionObject as Exception);
                    logger.Flush();
                };

                using (_game = new())
                {
                    _game.Run(60, 60);
                }
            }
        }

        public static void Restart()
        {
            _game.Exit();
            using (_game = new())
            {
                _game.Run(60, 60);
            }
        }
    }
}
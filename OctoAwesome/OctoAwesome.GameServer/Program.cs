using System;
using System.IO;
using System.Threading;
using OctoAwesome.Logging;
using OctoAwesome.Network;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        private static ManualResetEvent _manualResetEvent;
        private static ILogger _logger;

        private static void Main(string[] args)
        {
            using (var typeContainer = TypeContainer.Get<ITypeContainer>())
            {
                Startup.Register(typeContainer);
                Startup.ConfigureLogger(ClientType.GameServer);

                Network.Startup.Register(typeContainer);

                _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As("OctoAwesome.GameServer");
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    File.WriteAllText(
                        Path.Combine(".", "logs", $"server-dump-{DateTime.Now:ddMMyy_hhmmss}.txt"),
                        e.ExceptionObject.ToString());

                    _logger.Fatal($"Unhandled Exception: {e.ExceptionObject}", e.ExceptionObject as Exception);
                    _logger.Flush();
                };

                _manualResetEvent = new ManualResetEvent(false);

                _logger.Info("Server start");
                var fileInfo = new FileInfo(Path.Combine(".", "settings.json"));
                Settings settings;

                if (!fileInfo.Exists)
                {
                    _logger.Debug("Create new Default Settings");
                    settings = new Settings
                    {
                        FileInfo = fileInfo
                    };
                    settings.Save();
                }
                else
                {
                    _logger.Debug("Load Settings");
                    settings = new Settings(fileInfo);
                }


                typeContainer.Register(settings);
                typeContainer.Register<ISettings, Settings>(settings);
                typeContainer.Register<ServerHandler>(InstanceBehaviour.Singleton);
                typeContainer.Get<ServerHandler>().Start();

                Console.CancelKeyPress += (s, e) => _manualResetEvent.Set();
                _manualResetEvent.WaitOne();
                settings.Save();
            }
        }
    }
}
using System;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent _manualResetEvent;
        private static Logger _logger;

        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("octoawesome.logconsole"));
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile") { FileName = "server.log" });

            LogManager.Configuration = config;
            _logger = LogManager.GetCurrentClassLogger(typeof(Program));
            
            _manualResetEvent = new ManualResetEvent(false);

            _logger.Info("Server started");
            ServerHandler = new ServerHandler();
            ServerHandler.Start();

            Console.CancelKeyPress += (s, e) => _manualResetEvent.Set();
            _manualResetEvent.WaitOne();
        }
    }
}

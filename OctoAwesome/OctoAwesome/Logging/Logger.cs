using NLog;
using System;

namespace OctoAwesome.Logging
{
    public sealed class Logger : ILogger
    {
        private static readonly NLog.ILogger _nullLogger;

        private NLog.ILogger _internalLogger;

        static Logger() => _nullLogger = LogManager.LogFactory.CreateNullLogger();

        public Logger() => _internalLogger = _nullLogger;

        public void Info(string message) => _internalLogger.Info(message);

        public void Info(string message, Exception exception) => _internalLogger.Info(exception, message);

        public void Info<T>(T message) => _internalLogger.Info(message);

        public void Error(string message) => _internalLogger.Error(message);

        public void Error(string message, Exception exception) => _internalLogger.Error(exception, message);

        public void Error<T>(T message) => _internalLogger.Error(message);

        public void Warn(string message) => _internalLogger.Warn(message);

        public void Warn(string message, Exception exception) => _internalLogger.Warn(exception, message);

        public void Warn<T>(T message) => _internalLogger.Warn(message);

        public void Debug(string message) => _internalLogger.Debug(message);

        public void Debug(string message, Exception exception) => _internalLogger.Debug(exception, message);

        public void Debug<T>(T message) => _internalLogger.Debug(message);

        public void Trace(string message) => _internalLogger.Trace(message);

        public void Trace(string message, Exception exception) => _internalLogger.Trace(exception, message);

        public void Trace<T>(T message) => _internalLogger.Trace(message);

        public void Fatal(string message) => _internalLogger.Trace(message);

        public void Fatal(string message, Exception exception) => _internalLogger.Trace(exception, message);

        public void Fatal<T>(T message) => _internalLogger.Trace(message);

        public ILogger As(string loggerName)
        {
            _internalLogger = LogManager.GetLogger(loggerName);
            return this;
        }

        public ILogger As(Type type) => As(type.FullName);

        public void Flush() => LogManager.Flush();
    }
}
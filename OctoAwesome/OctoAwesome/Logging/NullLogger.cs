using System;

namespace OctoAwesome.Logging
{
    public class NullLogger : ILogger
    {
        static NullLogger()
        {
            Default = new NullLogger().As(nameof(Default));
        }

        public static ILogger Default { get; }

        public string Name { get; private set; }

        public ILogger As(string loggerName)
        {
            return new NullLogger
            {
                Name = loggerName
            };
        }

        public ILogger As(Type type)
        {
            return As(type.FullName);
        }

        public void Debug(string message)
        {
        }

        public void Debug(string message, Exception exception)
        {
        }

        public void Debug<T>(T message)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string message, Exception exception)
        {
        }

        public void Error<T>(T message)
        {
        }

        public void Fatal(string message)
        {
        }

        public void Fatal(string message, Exception exception)
        {
        }

        public void Fatal<T>(T message)
        {
        }

        public void Info(string message)
        {
        }

        public void Info(string message, Exception exception)
        {
        }

        public void Info<T>(T message)
        {
        }

        public void Trace(string message)
        {
        }

        public void Trace(string message, Exception exception)
        {
        }

        public void Trace<T>(T message)
        {
        }

        public void Warn(string message)
        {
        }

        public void Warn(string message, Exception exception)
        {
        }

        public void Warn<T>(T message)
        {
        }

        public void Flush()
        {
        }
    }
}
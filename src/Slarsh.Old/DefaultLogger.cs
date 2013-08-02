namespace Slarsh
{
    using System;

    public sealed class DefaultLogger : ILogger
    {
        private static readonly Lazy<DefaultLogger> DefaultLoggerLazyInstance = new Lazy<DefaultLogger>(false);

        public static DefaultLogger Instance
        {
            get
            {
                return DefaultLoggerLazyInstance.Value;
            }
        }

        public void Debug(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }

        public void WarnException(string message, Exception exception)
        {
        }

        public void Error(string message)
        {
        }

        public void ErrorException(string message, Exception exception)
        {
        }

        public void Fatal(string message)
        {
        }

        public void FatalException(string message, Exception exception)
        {
        }
    }
}

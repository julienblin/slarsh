namespace Slarsh
{
    using System;

    public interface ILogger
    {
        void Debug(string message);

        void Info(string message);

        void Warn(string message);

        void WarnException(string message, Exception exception);

        void Error(string message);

        void ErrorException(string message, Exception exception);

        void Fatal(string message);

        void FatalException(string message, Exception exception);
    }
}

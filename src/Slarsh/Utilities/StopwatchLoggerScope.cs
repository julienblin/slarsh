namespace Slarsh.Utilities
{
    using System;
    using System.Diagnostics;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Disposable trick - no resources.")]
    public class StopwatchLoggerScope : IDisposable
    {
        private readonly ILogger logger;

        private readonly string stoppingMessage;

        private readonly object obj;

        private readonly Stopwatch stopwatch;

        public StopwatchLoggerScope(ILogger logger, string startingMessage, string stoppingMessage, object obj)
        {
            this.logger = logger;
            this.stoppingMessage = stoppingMessage;
            this.obj = obj;
            this.logger.Debug(startingMessage.Format(this.obj));
            this.stopwatch = Stopwatch.StartNew();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Disposable trick - no resources.")]
        public void Dispose()
        {
            this.stopwatch.Stop();
            this.logger.Debug(this.stoppingMessage.Format(this.obj, this.stopwatch.ElapsedMilliseconds));
        }
    }
}

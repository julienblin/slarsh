namespace Slarsh.Utilities
{
    using System;
    using System.Diagnostics;

    using Common.Logging;

    /// <summary>
    /// Helper utility that logs the time taken to perform a block of code.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Disposable trick - no resources.")]
    public class StopwatchLogScope : IDisposable
    {
        /// <summary>
        /// The log.
        /// </summary>
        private readonly ILog log;

        /// <summary>
        /// The stopping message.
        /// </summary>
        private readonly string stoppingMessage;

        /// <summary>
        /// The target object.
        /// </summary>
        private readonly object obj;

        /// <summary>
        /// The stopwatch.
        /// </summary>
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchLogScope"/> class.
        /// </summary>
        /// <param name="log">
        /// The log.
        /// </param>
        /// <param name="startingMessage">
        /// The starting message. Format must contains {0}.
        /// </param>
        /// <param name="stoppingMessage">
        /// The stopping message. Format must contains {0} {1}.
        /// </param>
        /// <param name="obj">
        /// The target object. - will be use as actual {0} argument in logs. {1} will be the time taken in milliseconds.
        /// </param>
        public StopwatchLogScope(ILog log, string startingMessage, string stoppingMessage, object obj)
        {
            this.log = log;
            this.stoppingMessage = stoppingMessage;
            this.obj = obj;
            this.log.Debug(startingMessage.Format(this.obj));
            this.stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Stops the stopwatch and logs the time taken.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Disposable trick - no resources.")]
        public void Dispose()
        {
            this.stopwatch.Stop();
            this.log.Debug(this.stoppingMessage.Format(this.obj, this.stopwatch.ElapsedMilliseconds));
        }
    }
}

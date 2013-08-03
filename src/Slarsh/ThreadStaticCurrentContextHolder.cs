namespace Slarsh
{
    using System;

    /// <summary>
    /// <see cref="ICurrentContextHolder"/> implementation that stores <see cref="IContext"/> relative to current thread.
    /// </summary>
    public class ThreadStaticCurrentContextHolder : ICurrentContextHolder
    {
        /// <summary>
        /// The current context.
        /// </summary>
        [ThreadStatic]
        private static IContext threadCurrentContext;

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        public IContext GetCurrentContext()
        {
            return threadCurrentContext;
        }

        /// <summary>
        /// The set current context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void SetCurrentContext(IContext context)
        {
            threadCurrentContext = context;
        }
    }
}

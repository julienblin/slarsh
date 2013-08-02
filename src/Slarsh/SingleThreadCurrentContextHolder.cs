namespace Slarsh
{
    /// <summary>
    /// <see cref="ICurrentContextHolder"/> implementation for single-thread applications.
    /// Can only hold one context at a time.
    /// </summary>
    public class SingleThreadCurrentContextHolder : ICurrentContextHolder
    {
        /// <summary>
        /// The current context.
        /// </summary>
        private IContext currentContext;

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        public IContext GetCurrentContext()
        {
            return this.currentContext;
        }

        /// <summary>
        /// The set current context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void SetCurrentContext(IContext context)
        {
            this.currentContext = context;
        }
    }
}

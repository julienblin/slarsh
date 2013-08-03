namespace Slarsh
{
    /// <summary>
    /// Static convenience methods for <see cref="IContext"/>.
    /// </summary>
    public static class Context
    {
        /// <summary>
        /// Gets a value indicating whether there is a current context.
        /// </summary>
        public static bool HasCurrent
        {
            get
            {
                return ContextFactory.HasCurrentContext();
            }
        }

        /// <summary>
        /// Gets or sets the current context.
        /// </summary>
        public static IContext Current
        {
            get
            {
                return ContextFactory.GetCurrentContext();
            }

            set
            {
                ContextFactory.SetCurrentContext(value);
            }
        }
    }
}

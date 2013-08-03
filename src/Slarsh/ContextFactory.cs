namespace Slarsh
{
    using System;
    using System.Transactions;

    using Common.Logging;

    using Slarsh.Impl;

    /// <summary>
    /// Entry point for a <c>Slarsh</c> application.
    /// </summary>
    public static class ContextFactory
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The current context holder.
        /// </summary>
        private static ICurrentContextHolder currentContextHolder;

        /// <summary>
        /// The current context factory.
        /// </summary>
        private static ContextFactoryImpl currentContextFactory;

        /// <summary>
        /// Gets the current <see cref="IContextFactory"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "OK here - global problem.")]
        public static IContextFactory Current
        {
            get
            {
                if (currentContextFactory == null)
                {
                    throw new SlarshException(Resources.NoCurrentContextFactory);
                }

                return currentContextFactory;
            }
        }

        /// <summary>
        /// Starts the context factory. Must be called prior to anything.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="IContextFactory"/>.
        /// </returns>
        public static IContextFactory Start(ContextFactoryConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (currentContextFactory != null)
            {
                Log.Warn(Resources.DiscardingCurrentContextFactory);
                currentContextFactory.Dispose();
            }

            currentContextHolder = configuration.CurrentContextHolder;
            currentContextFactory = new ContextFactoryImpl(configuration);
            currentContextFactory.Start();
            return currentContextFactory;
        }

        /// <summary>
        /// Creates and starts a new context with default options (TransactionScopeOption.Required, new TransactionOptions()).
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        public static IContext StartNewContext()
        {
            return Current.StartNewContext();
        }

        /// <summary>
        /// Creates and starts a new context.
        /// </summary>
        /// <param name="transactionScopeOption">
        /// The transaction scope option.
        /// </param>
        /// <param name="transactionOptions">
        /// The transaction options.
        /// </param>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        public static IContext StartNewContext(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions)
        {
            return Current.StartNewContext(transactionScopeOption, transactionOptions);
        }

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        internal static IContext GetCurrentContext()
        {
            if (currentContextHolder == null)
            {
                throw new SlarshException(Resources.NoCurrentContextHolder);
            }

            return currentContextHolder.GetCurrentContext();
        }

        /// <summary>
        /// Sets the current context.
        /// </summary>
        /// <param name="context">
        /// The current context.
        /// </param>
        internal static void SetCurrentContext(IContext context)
        {
            if (currentContextHolder == null)
            {
                throw new SlarshException(Resources.NoCurrentContextHolder);
            }

            var currentContext = currentContextHolder.GetCurrentContext();
            if ((currentContext != null) && currentContext.IsReady)
            {
                throw new SlarshException(Resources.UnableToSetCurrentContextBecauseThereIsAlreadyOne);
            }

            currentContextHolder.SetCurrentContext(context);
        }
    }
}

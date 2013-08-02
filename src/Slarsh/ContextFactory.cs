namespace Slarsh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;

    using Common.Logging;

    using Slarsh.Utilities;

    /// <summary>
    /// The Context Factory is the main entry point for <c>Slarsh</c>.
    /// It must be unique per application container, usually configured and started at the beginning.
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        /// <summary>
        /// The sync root for thread safety.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The current context holder.
        /// </summary>
        private static ICurrentContextHolder currentContextHolder;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILog log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The context provider factories.
        /// </summary>
        private readonly List<IContextProviderFactory> contextProviderFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactory"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        protected ContextFactory(ContextFactoryConfiguration configuration)
        {
            configuration.EnforceValidation();
            this.contextProviderFactories = configuration.ContextProviderFactories.ToList();
            this.IsReady = false;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ContextFactory"/> class. 
        /// </summary>
        ~ContextFactory()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether the context factory is ready and can start new <see cref="IContext"/>.
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Gets the <see cref="IContextProviderFactory">Context provider factories</see>.
        /// </summary>
        public IEnumerable<IContextProviderFactory> ContextProviderFactories
        {
            get
            {
                return this.contextProviderFactories.AsReadOnly();
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

            currentContextHolder = configuration.CurrentContextHolder;
            var contextFactory = new ContextFactory(configuration);
            contextFactory.Start();
            return contextFactory;
        }

        /// <summary>
        /// Creates and starts a new context with default options (TransactionScopeOption.Required, new TransactionOptions()).
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        public IContext StartNewContext()
        {
            return this.StartNewContext(TransactionScopeOption.Required, new TransactionOptions());
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
        public IContext StartNewContext(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.CannotCreateContextIfFactoryNotStarted);
            }

            var context = new Context(this);
            this.log.Debug(Resources.Starting.Format(context));
            context.Start(transactionScopeOption, transactionOptions);
            SetCurrentContext(context);
            return context;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

            currentContextHolder.SetCurrentContext(context);
        }

        /// <summary>
        /// The starts the <see cref="IContextFactory"/>.
        /// Must be called prior to <see cref="IContextFactory.StartNewContext()"/>.
        /// </summary>
        protected void Start()
        {
            lock (SyncRoot)
            {
                if (this.IsReady)
                {
                    throw new SlarshException(Resources.CannotStartContextFactoryIfStarted);
                }

                using (new StopwatchLogScope(this.log, Resources.Starting, Resources.StartedIn, this))
                {
                    try
                    {
                        Parallel.ForEach(this.contextProviderFactories, factory => factory.Start(this));
                    }
                    catch (AggregateException ex)
                    {
                        this.log.Fatal(Resources.ErrorWhileStartingProviderFactories, ex);
                        throw new SlarshException(Resources.ErrorWhileStartingProviderFactories, ex);
                    }

                    this.IsReady = true;
                }
            }
        }

        /// <summary>
        /// Dispose appropriate resources.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources must be disposed, false otherwise.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parallel.ForEach(
                    this.ContextProviderFactories,
                    provider =>
                    {
                        try
                        {
                            provider.Dispose();
                        }
                        catch (Exception ex)
                        {
                            this.log.Warn(Resources.ErrorWhileDisposing.Format(provider), ex);
                        }
                    });
            }
        }
    }
}

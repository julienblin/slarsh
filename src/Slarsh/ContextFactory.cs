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
    /// Default implementation for <see cref="IContextFactory"/>.
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        /// <summary>
        /// The sync root for thread safety.
        /// </summary>
        private static readonly object SyncRoot = new object();

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
        /// <param name="contextProviderFactory">
        /// The main <see cref="IContextProviderFactory"/>.
        /// </param>
        public ContextFactory(IContextProviderFactory contextProviderFactory)
            : this(new[] { contextProviderFactory })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactory"/> class.
        /// </summary>
        /// <param name="contextProviderFactories">
        /// The list of <see cref="IContextProviderFactory"/>.
        /// </param>
        public ContextFactory(IEnumerable<IContextProviderFactory> contextProviderFactories)
        {
            this.contextProviderFactories = contextProviderFactories.ToList();
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
        /// The starts the <see cref="IContextFactory"/>.
        /// Must be called prior to <see cref="IContextFactory.StartNewContext()"/>.
        /// </summary>
        public void Start()
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

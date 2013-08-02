namespace Slarsh
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Slarsh.Utilities;

    public class ContextFactory : IContextFactory
    {
        /// <summary>
        /// The sync root for thread safety.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The context provider factories.
        /// </summary>
        private readonly List<IContextProviderFactory> contextProviderFactories; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactory"/> class.
        /// </summary>
        public ContextFactory()
        {
            this.contextProviderFactories = new List<IContextProviderFactory>();
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
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return this.logger ?? (this.logger = DefaultLogger.Instance);
            }
        }

        public bool IsReady { get; private set; }

        public IEnumerable<IContextProviderFactory> ContextProviderFactories
        {
            get
            {
                return new ReadOnlyCollection<IContextProviderFactory>(this.contextProviderFactories);
            }
        }

        public void Add(params IContextProviderFactory[] contextProviderFactories)
        {
            lock (SyncRoot)
            {
                if (this.IsReady)
                {
                    throw new SlarshException(Resources.CannotAddContextProvidersIfStarted);
                }

                foreach (var contextProviderFactory in contextProviderFactories)
                {
                    if (!this.contextProviderFactories.Contains(contextProviderFactory))
                    {
                        this.contextProviderFactories.Add(contextProviderFactory);
                    }
                }
            }
        }

        public void Start()
        {
            lock (SyncRoot)
            {
                if (this.IsReady)
                {
                    throw new SlarshException(Resources.CannotStartContextFactoryIfStarted);
                }

                using (new StopwatchLoggerScope(this.Logger, Resources.Starting, Resources.StartedIn, this))
                {
                    try
                    {
                        Parallel.ForEach(this.contextProviderFactories, factory => factory.Start(this));
                    }
                    catch (AggregateException ex)
                    {
                        this.Logger.FatalException(Resources.ErrorWhileStartingProviderFactories, ex);
                        throw new SlarshException(Resources.ErrorWhileStartingProviderFactories, ex);
                    }

                    this.IsReady = true;
                }
            }
        }

        public IContext CreateContext()
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.CannotCreateContextIfFactoryNotStarted);
            }

            this.Logger.Debug(Resources.CreatingContext);
            return new Context(this, this.contextProviderFactories.Select(x => x.CreateContextProvider()));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

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
                            this.Logger.WarnException(Resources.ErrorWhileDisposing.Format(provider), ex);
                        }
                    });
            }
        }
    }
}

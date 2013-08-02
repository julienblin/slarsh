namespace Slarsh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

    using Common.Logging;

    /// <summary>
    /// Default implementation for <see cref="IContext"/>.
    /// </summary>
    public class Context : IContext
    {
        /// <summary>
        /// The context providers.
        /// </summary>
        private readonly IEnumerable<IContextProvider> contextProviders;

        /// <summary>
        /// The context provider entity map cache.
        /// </summary>
        private readonly IDictionary<Type, IContextProvider> contextProviderEntityMapCache;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILog log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The transaction scope.
        /// </summary>
        private TransactionScope transactionScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        internal Context(IContextFactory contextFactory)
        {
            this.Id = Guid.NewGuid();
            this.IsReady = false;
            this.ContextFactory = contextFactory;
            this.contextProviders = contextFactory.ContextProviderFactories.Select(x => x.CreateContextProvider(this)).ToList();
            this.Values = new Dictionary<string, object>();
            this.contextProviderEntityMapCache = new Dictionary<Type, IContextProvider>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Context"/> class. 
        /// </summary>
        ~Context()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets the current context.
        /// </summary>
        public static IContext Current
        {
            get
            {
                return Slarsh.ContextFactory.GetCurrentContext();
            }

            set
            {
                Slarsh.ContextFactory.SetCurrentContext(value);
            }
        }

        /// <summary>
        /// Gets the unique id of this context.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the context is ready (started and not disposed or committed).
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Gets the <see cref="IContextFactory"/> that this context was created from.
        /// </summary>
        public IContextFactory ContextFactory { get; private set; }

        /// <summary>
        /// Gets the values dictionary associated with the context.
        /// Allows the storage of various contextual information.
        /// </summary>
        public IDictionary<string, object> Values { get; private set; }

        /// <summary>
        /// Adds an entity to the context.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Add(IEntity entity)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            this.GetContextProviderFor(entity.GetType()).Add(entity);
        }

        /// <summary>
        /// Removes an entity from the context.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Remove(IEntity entity)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            this.GetContextProviderFor(entity.GetType()).Remove(entity);
        }

        /// <summary>
        /// Gets an entity by its id.
        /// </summary>
        /// <param name="id">
        /// The entity id.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity.
        /// </typeparam>
        /// <returns>
        /// The entity.
        /// </returns>
        public T Get<T>(object id)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            return this.GetContextProviderFor(typeof(T)).Get<T>(id);
        }

        /// <summary>
        /// Creates a query.
        /// </summary>
        /// <typeparam name="T">
        /// The type of query to create.
        /// </typeparam>
        /// <returns>
        /// The query.
        /// </returns>
        public T CreateQuery<T>()
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            return this.GetContextProviderFor(typeof(T)).CreateQuery<T>();
        }

        /// <summary>
        /// Starts the context.
        /// </summary>
        /// <param name="transactionScopeOption">
        /// The transaction scope option.
        /// </param>
        /// <param name="transactionOptions">
        /// The transaction options.
        /// </param>
        public void Start(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions)
        {
            this.transactionScope = new TransactionScope(transactionScopeOption, transactionOptions);
            this.IsReady = true;
        }

        /// <summary>
        /// Commits the context and thus the underlying transaction.
        /// If a context is not committed before it is disposed, it will rollback.
        /// </summary>
        public void Commit()
        {
            this.log.Debug(Resources.Committing.Format(this));
            this.transactionScope.Dispose();
            this.IsReady = false;
        }

        /// <summary>
        /// Returns the <see cref="IContextProvider"/> suitable for the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The entity type.
        /// </param>
        /// <param name="throwException">
        /// True to throw an exception if not found, false otherwise.
        /// </param>
        /// <returns>
        /// The <see cref="IContextProvider"/>.
        /// </returns>
        /// <exception cref="SlarshException">
        /// If no suitable <see cref="IContextProvider"/> found.
        /// </exception>
        public IContextProvider GetContextProviderFor(Type type, bool throwException = true)
        {
            if (this.contextProviderEntityMapCache.ContainsKey(type))
            {
                return this.contextProviderEntityMapCache[type];
            }

            foreach (var contextProvider in this.contextProviders)
            {
                if (contextProvider.TakesCareOf(type))
                {
                    this.contextProviderEntityMapCache[type] = contextProvider;
                    return contextProvider;
                }
            }

            if (throwException)
            {
                throw new SlarshException(Resources.UnableToFindASuitableContextproviderFor.Format(type, string.Join(",", this.contextProviders)));
            }

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "Context({0})".Format(this.Id);
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
                this.log.Debug(Resources.Disposing.Format(this));

                foreach (var contextProvider in this.contextProviders)
                {
                    contextProvider.Dispose();
                }

                this.transactionScope.Dispose();
                this.IsReady = false;
                Current = null;
            }
        }
    }
}

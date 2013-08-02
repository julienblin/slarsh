namespace Slarsh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class Context : IContext
    {
        /// <summary>
        /// The context providers.
        /// </summary>
        private readonly IEnumerable<IContextProvider> contextProviders;

        private readonly IDictionary<Type, IContextProvider> contextProviderEntityMapCache;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        /// <param name="contextProviders">
        /// The context providers.
        /// </param>
        public Context(IContextFactory contextFactory, IEnumerable<IContextProvider> contextProviders)
        {
            this.IsReady = false;
            this.ContextFactory = contextFactory;
            this.contextProviders = contextProviders.ToList();
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

        public IContextFactory ContextFactory { get; private set; }

        public IDictionary<string, object> Values { get; private set; }

        public void Add(IEntity entity)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            this.GetContextProviderFor(entity.GetType()).Add(entity);
        }

        public void Remove(IEntity entity)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            this.GetContextProviderFor(entity.GetType()).Remove(entity);
        }

        public T Get<T>(object id)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            return this.GetContextProviderFor(typeof(T)).Get<T>(id);
        }

        public void Execute(IExecutable executable)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(IExecutable<T> executable)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            foreach (var contextProvider in this.contextProviders)
            {
                contextProvider.Start();
            }

            this.IsReady = true;
        }

        public void Commit()
        {
            foreach (var contextProvider in this.contextProviders)
            {
                contextProvider.Commit();
            }

            this.IsReady = false;
        }

        public void Rollback()
        {
            foreach (var contextProvider in this.contextProviders)
            {
                contextProvider.Rollback();
            }

            this.IsReady = false;
        }

        /// <summary>
        /// Returns the <see cref="IContextProvider"/> suitable for the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="IContextProvider"/>.
        /// </returns>
        /// <exception cref="SlarshException">
        /// If no suitable <see cref="IContextProvider"/> found.
        /// </exception>
        public IContextProvider GetContextProviderFor(Type type)
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

            throw new SlarshException(Resources.UnableToFindASuitableContextproviderFor.Format(type, string.Join(",", this.contextProviders)));
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
        /// Dispose appropriate resources.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources must be disposed, false otherwise.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var contextProvider in this.contextProviders)
                {
                    contextProvider.Dispose();
                }

                this.IsReady = false;
            }
        }
    }
}

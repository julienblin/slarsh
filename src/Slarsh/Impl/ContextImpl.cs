namespace Slarsh.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;

    using Common.Logging;

    /// <summary>
    /// Default implementation for <see cref="IContext"/>.
    /// </summary>
    internal class ContextImpl : IContext
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
        /// The dependent transaction.
        /// </summary>
        private DependentTransaction dependentTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextImpl"/> class.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        public ContextImpl(IContextFactory contextFactory)
        {
            this.Id = Guid.NewGuid();
            this.IsReady = false;
            this.ContextFactory = contextFactory;
            this.contextProviders = contextFactory.ContextProviderFactories.Select(x => x.CreateContextProvider(this)).ToList();
            this.Values = new Dictionary<string, object>();
            this.contextProviderEntityMapCache = new Dictionary<Type, IContextProvider>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextImpl"/> class by copy.
        /// Copies the Values and ContextFactory - everything else is new.
        /// </summary>
        /// <param name="parentContext">
        /// The parent context.
        /// </param>
        public ContextImpl(ContextImpl parentContext)
        {
            this.Id = Guid.NewGuid();
            this.Parent = parentContext;
            this.IsReady = false;
            this.ContextFactory = parentContext.ContextFactory;
            this.contextProviders = parentContext.ContextFactory.ContextProviderFactories.Select(x => x.CreateContextProvider(this)).ToList();
            this.Values = new Dictionary<string, object>(parentContext.Values);
            this.contextProviderEntityMapCache = new Dictionary<Type, IContextProvider>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ContextImpl"/> class. 
        /// </summary>
        ~ContextImpl()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// The transaction started event.
        /// </summary>
        public event GenericContextEventHandler TransactionStarted;

        /// <summary>
        /// The transaction committing event.
        /// </summary>
        public event GenericContextEventHandler TransactionCommitting;

        /// <summary>
        /// Gets the unique id of this context.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the parent context, if any.
        /// </summary>
        public IContext Parent { get; private set; }

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
        /// Fulfill a <see cref="IQuery{T}"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// The type or returned result query.
        /// </typeparam>
        /// <returns>
        /// The result.
        /// </returns>
        public T Fulfill<T>(IQuery<T> query)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            return this.GetContextProviderFor(query.GetType()).Fulfill(query);
        }

        /// <summary>
        /// Executes a command with no return type.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void Execute(ICommand command)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            command.Execute(this);
        }

        /// <summary>
        /// Executes a command and return the result.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <typeparam name="T">
        /// The type of result
        /// </typeparam>
        /// <returns>
        /// The result.
        /// </returns>
        public T Execute<T>(ICommand<T> command)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            return command.Execute(this);
        }

        /// <summary>
        /// Executes the command in an asynchronous manner.
        /// The command is executed in its own disposable cloned context, with a dependant transaction on the current transaction.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// A Task which executes the work.
        /// </returns>
        public Task ExecuteAsync(ICommand command)
        {
            return Task.Factory.StartNew(
                state =>
                {
                    var stateCmd = (AsyncTaskStateCommand)state;
                    try
                    {
                        stateCmd.Context.Start();
                        stateCmd.Context.Execute(stateCmd.Command);
                        stateCmd.Context.Commit();
                    }
                    finally
                    {
                        stateCmd.Context.Dispose();
                    }
                },
                new AsyncTaskStateCommand { Context = this.Clone(), Command = command });
        }

        /// <summary>
        /// Executes the command in an asynchronous manner.
        /// The command is executed in its own disposable cloned context, with a dependant transaction on the current transaction.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <typeparam name="T">
        /// The type of result
        /// </typeparam>
        /// <returns>
        /// A <see cref="Task{TResult}"/> which executes the work.
        /// </returns>
        public Task<T> ExecuteAsync<T>(ICommand<T> command)
        {
            return Task<T>.Factory.StartNew(
                state =>
                {
                    var stateCmd = (AsyncTaskStateCommandResult<T>)state;
                    try
                    {
                        stateCmd.Context.Start();
                        var result = stateCmd.Context.Execute(stateCmd.Command);
                        stateCmd.Context.Commit();
                        return result;
                    }
                    finally
                    {
                        stateCmd.Context.Dispose();
                    }
                },
                new AsyncTaskStateCommandResult<T> { Context = this.Clone(), Command = command });
        }

        /// <summary>
        /// Starts the context.
        /// </summary>
        public void Start()
        {
            this.log.Debug(Resources.Starting.Format(this));
            this.transactionScope = this.dependentTransaction != null ? new TransactionScope(this.dependentTransaction) : new TransactionScope();
            this.OnTransactionStarted(Transaction.Current);
            this.IsReady = true;
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
            if (this.dependentTransaction != null)
            {
                throw new SlarshException(Resources.TransactionOptionsCannotBeModified);
            }

            this.log.Debug(Resources.Starting.Format(this));
            this.transactionScope = new TransactionScope(transactionScopeOption, transactionOptions);
            this.OnTransactionStarted(Transaction.Current);
            this.IsReady = true;
        }

        /// <summary>
        /// Commits the context and thus the underlying transaction.
        /// If a context is not committed before it is disposed, it will rollback.
        /// </summary>
        public void Commit()
        {
            this.log.Debug(Resources.Committing.Format(this));
            this.OnTransactionCommitting();
            this.transactionScope.Complete();

            if (this.dependentTransaction != null)
            {
                this.dependentTransaction.Complete();
            }

            this.IsReady = false;
        }

        /// <summary>
        /// Clones the current context. Useful for multi-treaded scenarios.
        /// The cloned context is NOT started automatically.
        /// </summary>
        /// <param name="dependentTrans">
        /// True to have the context transaction dependent on the current transaction, false otherwise.
        /// </param>
        /// <returns>
        /// The cloned context.
        /// </returns>
        public IContext Clone(bool dependentTrans = true)
        {
            if (!this.IsReady)
            {
                throw new SlarshException(Resources.ContextIsNotReady);
            }

            var newContext = new ContextImpl(this);

            if (dependentTrans)
            {
                newContext.dependentTransaction = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);
            }

            return newContext;
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
                throw new SlarshException(Resources.UnableToFindASuitableContextProviderFor.Format(type, string.Join(",", this.contextProviders)));
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
            return this.Parent == null ? "Context({0})".Format(this.Id) : "Context({0} <- {1})".Format(this.Id, this.Parent.Id);
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

                if (this.dependentTransaction != null)
                {
                    this.dependentTransaction.Dispose();
                }

                this.IsReady = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="TransactionStarted"/> event.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        protected void OnTransactionStarted(Transaction transaction)
        {
            if (this.TransactionStarted != null)
            {
                this.TransactionStarted.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the <see cref="TransactionCommitting"/> event.
        /// </summary>
        protected void OnTransactionCommitting()
        {
            if (this.TransactionCommitting != null)
            {
                this.TransactionCommitting.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Arguments for async tasks commands.
        /// </summary>
        private struct AsyncTaskStateCommand
        {
            /// <summary>
            /// Gets or sets the context.
            /// </summary>
            public IContext Context { get; set; }

            /// <summary>
            /// Gets or sets the command.
            /// </summary>
            public ICommand Command { get; set; }
        }

        /// <summary>
        /// Arguments for async tasks commands.
        /// </summary>
        /// <typeparam name="T">
        /// The result type.
        /// </typeparam>
        private struct AsyncTaskStateCommandResult<T>
        {
            /// <summary>
            /// Gets or sets the context.
            /// </summary>
            public IContext Context { get; set; }

            /// <summary>
            /// Gets or sets the command.
            /// </summary>
            /// <typeparam name="T">
            /// The result type.
            /// </typeparam>
            public ICommand<T> Command { get; set; }
        }
    }
}

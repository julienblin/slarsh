namespace Slarsh
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Transactions;

    /// <summary>
    /// The generic context events handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void GenericContextEventHandler(object sender, EventArgs e);

    /// <summary>
    /// The Context interface.
    /// A Context is the main unit of work in <c>Slarsh</c>.
    /// All operations happened inside a context.
    /// When a context is started, a <see cref="TransactionScope"/> is also started, and committed or rolled back when disposed.
    /// Work is delegated to appropriate <see cref="IContextProvider"/> configured using <see cref="IContextProviderFactory"/>.
    /// Contexts are not thread-safe.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// The transaction started event.
        /// </summary>
        event GenericContextEventHandler TransactionStarted;

        /// <summary>
        /// The transaction committing event.
        /// </summary>
        event GenericContextEventHandler TransactionCommitting;

        /// <summary>
        /// Gets the unique id of this context.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the parent context, if any.
        /// </summary>
        IContext Parent { get; }

        /// <summary>
        /// Gets a value indicating whether the context is ready (started and not disposed or committed).
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Gets the <see cref="IContextFactory"/> that this context was created from.
        /// </summary>
        IContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets the values dictionary associated with the context.
        /// Allows the storage of various contextual information.
        /// </summary>
        IDictionary<string, object> Values { get; }

        /// <summary>
        /// Adds an entity to the context.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Add(IEntity entity);

        /// <summary>
        /// Removes an entity from the context.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Remove(IEntity entity);

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
        T Get<T>(object id);

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
        T Fulfill<T>(IQuery<T> query);

        /// <summary>
        /// Executes a command with no return type.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void Execute(ICommand command);

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
        T Execute<T>(ICommand<T> command);

        /// <summary>
        /// Executes the command with the result in an asynchronous manner.
        /// The command is executed in its own disposable cloned context, with a dependant transaction on the current transaction.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// A Task which executes the work.
        /// </returns>
        Task ExecuteAsync(ICommand command);

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
        Task<T> ExecuteAsync<T>(ICommand<T> command);

        /// <summary>
        /// Starts the context.
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the context.
        /// </summary>
        /// <param name="transactionScopeOption">
        /// The transaction scope option.
        /// </param>
        /// <param name="transactionOptions">
        /// The transaction options.
        /// </param>
        void Start(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions);

        /// <summary>
        /// Commits the context and thus the underlying transaction.
        /// If a context is not committed before it is disposed, it will rollback.
        /// </summary>
        void Commit();

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
        IContext Clone(bool dependentTrans = true);
    }
}

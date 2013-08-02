namespace Slarsh
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;

    /// <summary>
    /// The Context interface.
    /// A Context is the main unit of work in <c>Slarsh</c>.
    /// All operations happened inside a context.
    /// When a context is started, a <see cref="TransactionScope"/> is also started, and committed or rolled back when disposed.
    /// Work is delegated to appropriate <see cref="IContextProvider"/> configured using <see cref="IContextProviderFactory"/>.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Gets the unique id of this context.
        /// </summary>
        Guid Id { get; }

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
        /// Executes an <see cref="IExecutable"/>.
        /// </summary>
        /// <param name="executable">
        /// The executable.
        /// </param>
        void Execute(IExecutable executable);

        /// <summary>
        /// Executes an <see cref="IExecutable{T}"/> and return the results.
        /// </summary>
        /// <param name="executable">
        /// The executable.
        /// </param>
        /// <typeparam name="T">
        /// The type of the result.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Execute<T>(IExecutable<T> executable);

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
        IContextProvider GetContextProviderFor(Type type);
    }
}

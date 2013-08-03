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
        /// <param name="throwException">
        /// True to throw an exception if not found, false otherwise.
        /// </param>
        /// <returns>
        /// The <see cref="IContextProvider"/>.
        /// </returns>
        /// <exception cref="SlarshException">
        /// If no suitable <see cref="IContextProvider"/> found.
        /// </exception>
        IContextProvider GetContextProviderFor(Type type, bool throwException = true);
    }
}

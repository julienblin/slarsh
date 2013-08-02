namespace Slarsh
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;

    /// <summary>
    /// The Context Factory is the main entry point for <c>Slarsh</c>.
    /// It must be unique per application container, usually configured and started at the beginning.
    /// </summary>
    public interface IContextFactory : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the context factory is ready and can start new <see cref="IContext"/>.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Gets the <see cref="IContextProviderFactory">Context provider factories</see>.
        /// </summary>
        IEnumerable<IContextProviderFactory> ContextProviderFactories { get; }

        /// <summary>
        /// The starts the <see cref="IContextFactory"/>.
        /// Must be called prior to <see cref="StartNewContext()"/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Creates and starts a new context with default options (TransactionScopeOption.Required, new TransactionOptions()).
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        IContext StartNewContext();

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
        IContext StartNewContext(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions);
    }
}

namespace Slarsh
{
    using System;
    using System.Collections.Generic;

    public interface IContext : IDisposable
    {
        bool IsReady { get; }

        IContextFactory ContextFactory { get; }

        void Add(IEntity entity);

        void Remove(IEntity entity);

        T Get<T>(object id);

        void Execute(IExecutable executable);

        T Execute<T>(IExecutable<T> executable);

        IDictionary<string, object> Values { get; }

        void Start();

        void Commit();

        void Rollback();

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

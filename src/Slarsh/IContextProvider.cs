namespace Slarsh
{
    using System;

    /// <summary>
    /// The ContextProvider interface.
    /// A context provider provides the underlying persistence mechanism for some entities through contexts.
    /// </summary>
    public interface IContextProvider : IDisposable
    {
        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Add(IEntity entity);

        /// <summary>
        /// Removes an entity.
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
        /// Indicates whether this context provider takes care of the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// True if it can handle it, false otherwise.
        /// </returns>
        bool TakesCareOf(Type type);
    }
}

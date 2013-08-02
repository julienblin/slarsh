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
        /// Creates a query.
        /// </summary>
        /// <typeparam name="T">
        /// The type of query to create.
        /// </typeparam>
        /// <returns>
        /// The query.
        /// </returns>
        T CreateQuery<T>();

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

namespace Slarsh.NHibernate
{
    using System;

    using NH = global::NHibernate;

    /// <summary>
    /// A siamese query allow the creation of a query based on an actual entity properties.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the entity.
    /// </typeparam>
    public class NHSiameseQuery<T> : IQuery<IPreparedQuery<T>>, INHQuery
        where T : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NHSiameseQuery{T}"/> class.
        /// </summary>
        public NHSiameseQuery()
        {
            this.Values = Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Gets the siamese object.
        /// </summary>
        public T Values { get; private set; }

        /// <summary>
        /// Fulfills the query - call by the NHibernate <see cref="IContextProvider"/>.
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        public object Fulfill(IContextProvider contextProvider, NH.ISession session)
        {
            throw new System.NotImplementedException();
        }
    }
}

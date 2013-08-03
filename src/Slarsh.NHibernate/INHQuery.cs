﻿namespace Slarsh.NHibernate
{
    using global::NHibernate;

    /// <summary>
    /// Marker interface for NHibernate queries.
    /// </summary>
    public interface INHQuery
    {
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
        object Fulfill(IContextProvider contextProvider, ISession session);
    }
}

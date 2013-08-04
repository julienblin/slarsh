namespace Slarsh.NHibernate
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using NH = global::NHibernate;

    /// <summary>
    /// A dynamic query, that uses dynamic objects to create queries based on entities.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity and the result type for <see cref="IPreparedQuery{T}"/>.
    /// </typeparam>
    public class NHDynamicQuery<T> : DynamicObject, IQuery<IPreparedQuery<T>>, INHQuery
        where T : class, IEntity
    {
        private readonly Dictionary<PropertyInfo, NHDynamicQueryProperty> propertyAssignations = new Dictionary<PropertyInfo, NHDynamicQueryProperty>();

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
            return this.BuildPreparedQuery(contextProvider, session);
        }

        /// <summary>
        /// Builds the <see cref="IPreparedQuery{T}"/> by calling <see cref="BuildCriteria"/>
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="IPreparedQuery{T}"/>.
        /// </returns>
        protected virtual IPreparedQuery<T> BuildPreparedQuery(IContextProvider contextProvider, NH.ISession session)
        {
            return new PreparedQueryCriteria<T>(session, this.BuildCriteria(contextProvider, session));
        }

        /// <summary>
        /// Builds the <see cref="NH.ICriteria"/>.
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="NH.IQueryOver{TRoot,TSubtype}"/>.
        /// </returns>
        protected virtual NH.ICriteria BuildCriteria(IContextProvider contextProvider, NH.ISession session)
        {
            var query = session.CreateCriteria<T>();

            foreach (var propertyAssignation in this.propertyAssignations.Values)
            {
                propertyAssignation.Apply(query, session);
            }

            return query;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = typeof(T).GetProperty(binder.Name);
            if (property == null)
            {
                throw new SlarshException(string.Format(Resources.UnableToFindPropertyOnType, binder.Name, typeof(T)));
            }

            if (!this.propertyAssignations.ContainsKey(property))
            {
                this.propertyAssignations[property] = new NHDynamicQueryProperty(property, binder);
            }

            result = this.propertyAssignations[property];
            return true;
        }
    }
}

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
        /// <summary>
        /// The property assignations.
        /// </summary>
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
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = typeof(T).GetProperty(binder.Name);
            if (property == null)
            {
                throw new SlarshException(string.Format(Resources.UnableToFindPropertyOnType, binder.Name, typeof(T)));
            }

            if (!this.propertyAssignations.ContainsKey(property))
            {
                this.propertyAssignations[property] = new NHDynamicQueryProperty(property);
            }

            result = this.propertyAssignations[property];
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Convenience method for DynamicProperty.Eq().
            var property = typeof(T).GetProperty(binder.Name);
            if (property != null)
            {
                if (!this.propertyAssignations.ContainsKey(property))
                {
                    this.propertyAssignations[property] = new NHDynamicQueryProperty(property);
                }

                this.propertyAssignations[property].Eq(value);
                return true;
            }

            // Trying convenience setters.
            var convenienceSetterMatch = NHDynamicQueryProperty.MatchConvenienceSetters.Match(binder.Name);
            if (!convenienceSetterMatch.Success)
            {
                throw new SlarshException(string.Format(Resources.UnableToFindPropertyOnType, binder.Name, typeof(T)));
            }

            var propertyName = convenienceSetterMatch.Groups["propertyName"].Value;
            var method = convenienceSetterMatch.Groups["method"].Value;
            property = typeof(T).GetProperty(propertyName);
            if (property == null)
            {
                throw new SlarshException(string.Format(Resources.UnableToFindPropertyOnType, propertyName, typeof(T)));
            }

            if (!this.propertyAssignations.ContainsKey(property))
            {
                this.propertyAssignations[property] = new NHDynamicQueryProperty(property);
            }

            var convenienceMethod = typeof(NHDynamicQueryProperty).GetMethods().FirstOrDefault(x => (x.Name == method) && (x.GetParameters().Length == 1) && !x.ContainsGenericParameters);
            if (convenienceMethod == null)
            {
                throw new SlarshException(string.Format(Resources.UnableToFindConvenienceMethod, method, value.GetType()));
            }

            convenienceMethod.Invoke(this.propertyAssignations[property], new[] { value });
            return true;
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
    }
}

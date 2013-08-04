namespace Slarsh.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Text.RegularExpressions;

    using NH = global::NHibernate;

    /// <summary>
    /// A siamese query allow the creation of a query based on an actual entity properties.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the entity.
    /// </typeparam>
    public class NHSiameseQuery<T> : DynamicObject, IQuery<IPreparedQuery<T>>, INHQuery
        where T : class, IEntity
    {
        private static readonly List<PropertyRule> PropertyRules = new List<PropertyRule>
        {
            new PropertyRule(Regexes.PropertyName, (query, propertyName, setMemberValue) => query.Where(NH.Criterion.Restrictions.Eq(propertyName, setMemberValue.Value))),
            new PropertyRule(Regexes.InsensitiveLike, (query, propertyName, setMemberValue) => query.Where(NH.Criterion.Restrictions.InsensitiveLike(propertyName, (string)setMemberValue.Value, NH.Criterion.MatchMode.Anywhere))),
            new PropertyRule(Regexes.Like, (query, propertyName, setMemberValue) => query.Where(NH.Criterion.Restrictions.Like(propertyName, (string)setMemberValue.Value, NH.Criterion.MatchMode.Anywhere))),
            new PropertyRule(Regexes.Gt, (query, propertyName, setMemberValue) => query.Where(NH.Criterion.Restrictions.Gt(propertyName, setMemberValue.Value)))
        };

        private List<SetMemberValue> setMemberValues = new List<SetMemberValue>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.setMemberValues.Add(new SetMemberValue { Binder = binder, Value = value });
            return true;
        }

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

        protected virtual IPreparedQuery<T> BuildPreparedQuery(IContextProvider contextProvider, NH.ISession session)
        {
            return new PreparedQuery<T>(session, this.BuildQueryOver(contextProvider, session));
        }

        protected virtual NH.IQueryOver<T, T> BuildQueryOver(IContextProvider contextProvider, NH.ISession session)
        {
            var query = session.QueryOver<T>();

            this.ApplySetMemberValues(query, this.setMemberValues);

            return query;
        }

        protected virtual void ApplySetMemberValues(NH.IQueryOver<T, T> query, List<SetMemberValue> setMemberValues)
        {
            var targetType = typeof(T);
            foreach (var setMemberValue in setMemberValues)
            {
                var understood = false;

                foreach (var propertyRule in PropertyRules)
                {
                    var regexMatch = propertyRule.Regex.Match(setMemberValue.Binder.Name);
                    if (!regexMatch.Success)
                    {
                        continue;
                    }

                    var property = targetType.GetProperty(regexMatch.Groups["propertyName"].Value);
                    if (property == null)
                    {
                        continue;
                    }

                    propertyRule.Action(query, property.Name, setMemberValue);
                    understood = true;
                }

                if (!understood)
                {
                    throw new SlarshException(string.Format(Resources.UnableToInterpreteSiameseSetMember, setMemberValue.Binder.Name, setMemberValue.Value));
                }
            }
        }

        protected class SetMemberValue
        {
            public SetMemberBinder Binder { get; set; }

            public object Value { get; set; }
        }

        private class PropertyRule
        {
            public Regex Regex { get; private set; }

            public Action<NH.IQueryOver<T, T>, string, SetMemberValue> Action { get; private set; }

            public PropertyRule(Regex regex, Action<NH.IQueryOver<T, T>, string, SetMemberValue> action)
            {
                this.Regex = regex;
                this.Action = action;
            }
        }

        private static class Regexes
        {
            public static readonly Regex PropertyName = new Regex("^(?<propertyName>.+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

            public static readonly Regex InsensitiveLike = new Regex("^(?<propertyName>.+)InsensitiveLike$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

            public static readonly Regex Like = new Regex("^(?<propertyName>.+)Like$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

            public static readonly Regex Gt = new Regex("^(?<propertyName>.+)Gt$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);
        }
    }
}

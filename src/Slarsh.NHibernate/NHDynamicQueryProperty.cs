namespace Slarsh.NHibernate
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using global::NHibernate;
    using global::NHibernate.Criterion;
    using global::NHibernate.SqlCommand;

    public class NHDynamicQueryProperty : DynamicObject
    {
        private readonly PropertyInfo propertyInfo;

        private readonly GetMemberBinder binder;

        private readonly List<ICriterion> criterions = new List<ICriterion>();

        private readonly Dictionary<PropertyInfo, NHDynamicQueryProperty> joins = new Dictionary<PropertyInfo, NHDynamicQueryProperty>();

        public NHDynamicQueryProperty(PropertyInfo propertyInfo, GetMemberBinder binder)
        {
            this.propertyInfo = propertyInfo;
            this.binder = binder;
        }

        public virtual void Apply(ICriteria criteria, ISession session)
        {
            foreach (var criterion in this.criterions)
            {
                criteria.Add(criterion);
            }

            if (this.joins.Any())
            {
                var joinCriteria = criteria.CreateCriteria(this.propertyInfo.Name, JoinType.InnerJoin);
                foreach (var join in this.joins.Values)
                {
                    join.Apply(joinCriteria, session);
                }
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (typeof(IEnumerable).IsAssignableFrom(this.propertyInfo.PropertyType)
                && this.propertyInfo.PropertyType.IsGenericType)
            {
                // Handles enumerable associations
                var targetType = this.propertyInfo.PropertyType.GetGenericArguments()[0];
                var targetProperty = targetType.GetProperty(binder.Name);
                if (targetProperty == null)
                {
                    throw new SlarshException(string.Format(Resources.UnableToFindPropertyOnType, binder.Name, targetType));
                }

                if (!this.joins.ContainsKey(targetProperty))
                {
                    this.joins[targetProperty] = new NHDynamicQueryProperty(targetProperty, binder);
                }

                result = this.joins[targetProperty];
                return true;
            }

            throw new SlarshException(string.Format(Resources.UnableToFindPropertyOnType, binder.Name, this.propertyInfo.PropertyType));
        }

        public virtual void Eq(object value)
        {
            this.criterions.Add(Restrictions.Eq(this.propertyInfo.Name, value));
        }

        public virtual void Like(string value, MatchMode matchMode = null)
        {
            if (matchMode == null)
            {
                matchMode = MatchMode.Anywhere;
            }

            this.criterions.Add(Restrictions.Like(this.propertyInfo.Name, value, matchMode));
        }

        public virtual void InsensitiveLike(string value, MatchMode matchMode = null)
        {
            if (matchMode == null)
            {
                matchMode = MatchMode.Anywhere;
            }

            this.criterions.Add(Restrictions.InsensitiveLike(this.propertyInfo.Name, value, matchMode));
        }

        public virtual void Gt(object value)
        {
            this.criterions.Add(Restrictions.Gt(this.propertyInfo.Name, value));
        }

        public virtual void Ge(object value)
        {
            this.criterions.Add(Restrictions.Ge(this.propertyInfo.Name, value));
        }

        public virtual void Lt(object value)
        {
            this.criterions.Add(Restrictions.Lt(this.propertyInfo.Name, value));
        }

        public virtual void Le(object value)
        {
            this.criterions.Add(Restrictions.Le(this.propertyInfo.Name, value));
        }

        public virtual void Between(object lo, object hi)
        {
            this.criterions.Add(Restrictions.Between(this.propertyInfo.Name, lo, hi));
        }

        public virtual void In(object value, params object[] values)
        {
            var collection = new ArrayList { value };
            if (values != null)
            {
                collection.AddRange(values);
            }

            this.In((ICollection)values);
        }

        public virtual void In(object[] values)
        {
            this.In((ICollection)values);
        }

        public virtual void In<U>(IEnumerable<U> values)
        {
            this.In((ICollection)values);
        }

        public virtual void In(ICollection values)
        {
            this.criterions.Add(Restrictions.In(this.propertyInfo.Name, values));
        }

        public virtual void IsNull(bool isNull = true)
        {
            this.criterions.Add(
                isNull ? Restrictions.IsNull(this.propertyInfo.Name) : Restrictions.IsNotNull(this.propertyInfo.Name));
        }

        public virtual void IsNotNull()
        {
            this.criterions.Add(Restrictions.IsNotNull(this.propertyInfo.Name));
        }

        public virtual void IsEmpty(bool isEmpty = true)
        {
            this.criterions.Add(
                isEmpty ? Restrictions.IsEmpty(this.propertyInfo.Name) : Restrictions.IsNotEmpty(this.propertyInfo.Name));
        }

        public virtual void IsNotEmpty()
        {
            this.criterions.Add(Restrictions.IsNotEmpty(this.propertyInfo.Name));
        }
    }
}

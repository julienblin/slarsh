﻿namespace SampleApp.Core.Queries
{
    using NHibernate;
    using NHibernate.Criterion;

    using SampleApp.Core.Entities;

    using Slarsh;
    using Slarsh.NHibernate;

    public class EmployeeQuery : NHQueryQueryOver<Employee>
    {
        public string FirstNameLike { get; set; }

        public string LastNameLike { get; set; }

        protected override IQueryOver<Employee, Employee> BuildQueryOver(IContextProvider contextProvider, ISession session)
        {
            var query = session.QueryOver<Employee>();

            if (!string.IsNullOrEmpty(this.FirstNameLike))
            {
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Employee>(x => x.FirstName), this.FirstNameLike, MatchMode.Anywhere));
            }

            if (!string.IsNullOrEmpty(this.LastNameLike))
            {
                query.Where(
                    Restrictions.InsensitiveLike(
                        Projections.Property<Employee>(x => x.LastName), this.LastNameLike, MatchMode.Anywhere));
            }

            return query;
        }
    }
}

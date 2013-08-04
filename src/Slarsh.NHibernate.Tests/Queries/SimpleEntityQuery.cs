namespace Slarsh.NHibernate.Tests.Queries
{
    using global::NHibernate;

    using global::NHibernate.Criterion;

    using Slarsh.NHibernate.Tests.Entities;

    public class SimpleEntityQuery : NHQueryQueryOver<Employee>
    {
        public string NameLike { get; set; }

        protected override IQueryOver<Employee, Employee> BuildQueryOver(IContextProvider contextProvider, ISession session)
        {
            var query = session.QueryOver<Employee>();

            if (!string.IsNullOrEmpty(this.NameLike))
            {
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Employee>(x => x.Name), this.NameLike, MatchMode.Anywhere));
            }

            return query;
        }
    }
}

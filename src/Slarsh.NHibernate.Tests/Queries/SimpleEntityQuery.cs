namespace Slarsh.NHibernate.Tests.Queries
{
    using global::NHibernate;

    using global::NHibernate.Criterion;

    using Slarsh.NHibernate.Tests.Entities;

    public class SimpleEntityQuery : NHQuery<SimpleEntity>
    {
        public SimpleEntityQuery(ISession session)
            : base(session)
        {
        }

        public string NameLike { get; set; }

        protected override IQueryOver<SimpleEntity, SimpleEntity> CreateQueryOver(ISession session)
        {
            var query = session.QueryOver<SimpleEntity>();

            if (!string.IsNullOrEmpty(this.NameLike))
            {
                query.Where(Restrictions.InsensitiveLike(Projections.Property<SimpleEntity>(x => x.Name), this.NameLike, MatchMode.Anywhere));
            }

            return query;
        }
    }
}

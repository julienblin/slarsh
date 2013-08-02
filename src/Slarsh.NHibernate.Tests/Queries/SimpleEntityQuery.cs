namespace Slarsh.NHibernate.Tests.Queries
{
    using global::NHibernate;

    using global::NHibernate.Criterion;

    using Slarsh.NHibernate.Tests.Entities;

    public class SimpleEntityQuery : NHEnumerableQuery<SimpleEntity>
    {
        public string NameLike { get; set; }

        protected override IQueryOver<SimpleEntity, SimpleEntity> CreateNHQuery(ISession nhsession)
        {
            var query = nhsession.QueryOver<SimpleEntity>();

            if (!string.IsNullOrEmpty(this.NameLike))
            {
                query.Where(Restrictions.InsensitiveLike(Projections.Property<SimpleEntity>(x => x.Name), NameLike, MatchMode.Anywhere));
            }

            return query;
        }
    }
}

namespace Slarsh.NHibernate.Tests
{
    using System.Collections.Generic;

    using FluentAssertions;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;
    using Slarsh.NHibernate.Tests.Queries;

    using NHProperty = global::NHibernate.Cfg.Environment;

    [TestFixture]
    public class SimpleTest
    {
        [Test]
        public void It_should_perform_a_basic_persistance_test()
        {
            using (var contextFactory = ContextFactory.Start(new ContextFactoryConfiguration(SetupFixture.CreateNHContextProviderFactory())))
            using (var context = contextFactory.StartNewContext())
            {
                var simpleEntity = new SimpleEntity { Name = "Foo" };
                context.Add(simpleEntity);

                var entity = context.Get<SimpleEntity>(simpleEntity.Id);

                entity.Should().Be(simpleEntity);
            }
        }

        [Test]
        public void It_should_perform_a_basic_query()
        {
            using (var contextFactory = ContextFactory.Start(new ContextFactoryConfiguration(SetupFixture.CreateNHContextProviderFactory())))
            using (var context = contextFactory.StartNewContext())
            {
                var simpleEntity = new SimpleEntity { Name = "Foo" };
                context.Add(simpleEntity);

                var query = context.CreateQuery<SimpleEntityQuery>();
                query.NameLike = "F";
                query.OrderBy(x => x.Name);

                var queryResult = query.Paginate();
                queryResult.TotalItems.Should().Be(1);
                queryResult.CurrentPage.Should().Be(1);
                queryResult.PageSize.Should().BeGreaterThan(0);
                queryResult.Result.Should().HaveCount(1);
            }
        }
    }
}

namespace Slarsh.NHibernate.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;
    using Slarsh.NHibernate.Tests.Queries;

    [TestFixture]
    public class SimpleTest
    {
        [Test]
        public void It_should_perform_a_basic_persistance_test()
        {
            using (var context = ContextFactory.StartNewContext())
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
            using (var context = ContextFactory.StartNewContext())
            {
                var simpleEntity = new SimpleEntity { Name = "Foo" };
                context.Add(simpleEntity);

                var query = new SimpleEntityQuery { NameLike = "F" };
                var queryResult = context.Fulfill(query).Paginate();

                queryResult.TotalItems.Should().Be(1);
                queryResult.CurrentPage.Should().Be(1);
                queryResult.PageSize.Should().BeGreaterThan(0);
                queryResult.Result.Should().HaveCount(1);
            }
        }
    }
}

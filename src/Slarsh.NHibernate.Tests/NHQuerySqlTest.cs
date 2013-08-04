namespace Slarsh.NHibernate.Tests
{
    using System.Linq;

    using FluentAssertions;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;
    using Slarsh.NHibernate.Tests.Queries;

    [TestFixture]
    public class NHQuerySqlTest
    {
        [Test]
        public void It_should_execute_SQL()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });

                var result = context.Fulfill(new SqlQuery());

                result.Should().HaveCount(1);
                result.First().Name.Should().Be("Foo");
            }
        }
    }
}

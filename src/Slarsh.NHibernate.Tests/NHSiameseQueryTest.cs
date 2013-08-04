namespace Slarsh.NHibernate.Tests
{
    using System.Collections.Generic;

    using FluentAssertions;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;

    [TestFixture]
    public class NHSiameseQueryTest
    {
        [Test]
        public void It_should_filter_simple_assignations()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                context.Add(new SimpleEntity { Name = "Foo", Age = 25 });
                context.Add(new SimpleEntity { Name = "Bar", Age = 30 });

                dynamic query = new NHSiameseQuery<SimpleEntity>();
                query.Name = "Foo";
                ((int)context.Fulfill(query).Count()).Should().Be(1);

                query = new NHSiameseQuery<SimpleEntity>();
                query.Age = 25;
                ((int)context.Fulfill(query).Count()).Should().Be(1);
            }
        }

        [Test]
        public void It_should_filter_Like_assignations()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                context.Add(new SimpleEntity { Name = "Foo" });
                context.Add(new SimpleEntity { Name = "FooBar" });

                dynamic query = new NHSiameseQuery<SimpleEntity>();
                query.NameLike = "Foo";
                ((int)context.Fulfill(query).Count()).Should().Be(2);

                query = new NHSiameseQuery<SimpleEntity>();
                query.NameLike = "Bar";
                ((int)context.Fulfill(query).Count()).Should().Be(1);
            }
        }

        [Test]
        public void It_should_filter_InsensitiveLike_assignations()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                context.Add(new SimpleEntity { Name = "Foo" });
                context.Add(new SimpleEntity { Name = "FooBar" });

                dynamic query = new NHSiameseQuery<SimpleEntity>();
                query.NameInsensitiveLike = "foo";
                ((int)context.Fulfill(query).Count()).Should().Be(2);

                query = new NHSiameseQuery<SimpleEntity>();
                query.NameInsensitiveLike = "bar";
                ((int)context.Fulfill(query).Count()).Should().Be(1);
            }
        }

        [Test]
        public void It_should_filter_gt_assignations()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                context.Add(new SimpleEntity { Name = "Foo", Age = 25 });
                context.Add(new SimpleEntity { Name = "Bar", Age = 30 });

                dynamic query = new NHSiameseQuery<SimpleEntity>();
                query.AgeGt = 20;
                ((int)context.Fulfill(query).Count()).Should().Be(2);

                query = new NHSiameseQuery<SimpleEntity>();
                query.AgeGt = 27;
                ((int)context.Fulfill(query).Count()).Should().Be(1);
            }
        }
    }
}

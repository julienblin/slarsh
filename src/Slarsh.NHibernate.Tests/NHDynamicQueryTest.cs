namespace Slarsh.NHibernate.Tests
{
    using System;

    using FluentAssertions;

    using global::NHibernate.Criterion;

    using NUnit.Framework;

    using Slarsh.NHibernate.Tests.Entities;

    [TestFixture]
    public class NHDynamicQueryTest
    {
        [Test]
        public void It_should_filter_Eq()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo", Age = 25 });
                context.Add(new Employee { Name = "Bar", Age = 30 });

                dynamic query = new NHDynamicQuery<Employee>();
                query.Name.Eq("Foo");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Age.Eq(25);
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query.Name.Eq("Bar");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(0);
            }
        }

        [Test]
        public void It_should_filter_Like_and_InsensitiveLike()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "FooBar" });

                dynamic query = new NHDynamicQuery<Employee>();
                query.Name.Like("Foo");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(2);

                query = new NHDynamicQuery<Employee>();
                query.Name.Like("Foo", MatchMode.Start);
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(2);

                query = new NHDynamicQuery<Employee>();
                query.Name.Like("Foo", MatchMode.End);
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Name.InsensitiveLike("foo");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(2);

                query = new NHDynamicQuery<Employee>();
                query.Name.InsensitiveLike("bar");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);
            }
        }

        [Test]
        public void It_should_handle_joins()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                var now = DateTime.Now;

                var employee = new Employee { Name = "Foo" };
                context.Add(employee);
                context.Add(new Employee { Name = "FooBar" });

                var vacancy = new Vacancy { Employee = employee, StartDate = now, EndDate = now.AddDays(5) };
                context.Add(vacancy);

                dynamic query = new NHDynamicQuery<Employee>();
                query.Vacancies.StartDate.Gt(now.AddDays(-1));
                query.Vacancies.EndDate.Lt(now.AddDays(6));

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);
            }
        }

        [Test]
        public void It_should_handle_references()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                var boss = new Employee { Name = "The Boss." };
                context.Add(boss);

                var subordinate = new Employee { Name = "subordinate", Boss = boss };
                context.Add(subordinate);

                context.Add(new Employee { Name = "another one" });

                dynamic query = new NHDynamicQuery<Employee>();
                query.Boss.IsNotNull();

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Boss.Name.Eq("The Boss.");
                query.Boss.Boss.IsNull();

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);
            }
        }

        [Test]
        public void It_should_handle_assignations_and_convenience_setters()
        {
            using (var context = ContextFactory.StartNewContext())
            {
                var now = DateTime.Now;

                var boss = new Employee { Name = "The Boss.", Age = 45 };
                context.Add(boss);

                var subordinate = new Employee { Name = "subordinate", Boss = boss };
                context.Add(subordinate);

                var vacancy = new Vacancy { Employee = boss, StartDate = now, EndDate = now.AddDays(5) };
                context.Add(vacancy);

                context.Add(new Employee { Name = "Foo", Age = 25 });
                context.Add(new Employee { Name = "Bar", Age = 30 });
                
                dynamic query = new NHDynamicQuery<Employee>();
                query.Age = 25;

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.AgeGt = 27;

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(2);

                query = new NHDynamicQuery<Employee>();
                query.NameLike = "Bar";

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.AgeBetween = new object[] { 20, 28 };

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.AgeIn = new[] { 20, 28, 30 };

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Boss.Name = boss.Name;

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Boss.AgeGt = 40;

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Boss.Vacancies.StartDateGt = now.AddDays(-1);

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Boss.Vacancies.EndDateGt = now.AddDays(10);

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(0);
            }
        }
    }
}

namespace Slarsh.NHibernate.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class NHContextProviderFactoryTest
    {
        [Test]
        public void It_should_validate_connection_string()
        {
            var nhContextProviderFactory = new NHContextProviderFactory(
                new NHContextProviderFactoryConfiguration
                    {
                        DatabaseType = DatabaseType.SqLite
                    });

            nhContextProviderFactory.IsValid().Should().BeFalse();
        }
    }
}

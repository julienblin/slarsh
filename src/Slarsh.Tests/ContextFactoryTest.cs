namespace Slarsh.Tests
{
    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ContextFactoryTest
    {
        [Test]
        public void It_should_start_all_context_provider_factories()
        {
            var contextProviderFactory1 = new Mock<IContextProviderFactory>();
            contextProviderFactory1.Setup(x => x.Start(null));
            var contextProviderFactory2 = new Mock<IContextProviderFactory>();
            contextProviderFactory2.Setup(x => x.Start(null));

            using (var contextFactory = new ContextFactory(new[] { contextProviderFactory1.Object, contextProviderFactory2.Object }))
            {
                contextFactory.Start();

                contextProviderFactory1.Verify(x => x.Start(It.IsAny<IContextFactory>()));
                contextProviderFactory2.Verify(x => x.Start(It.IsAny<IContextFactory>()));
            }
        }

        [Test]
        public void It_should_dispose_all_context_provider_factories()
        {
            var contextProviderFactory1 = new Mock<IContextProviderFactory>();
            contextProviderFactory1.Setup(x => x.Dispose());
            var contextProviderFactory2 = new Mock<IContextProviderFactory>();
            contextProviderFactory2.Setup(x => x.Dispose());
            var contextFactory =
                new ContextFactory(new[] { contextProviderFactory1.Object, contextProviderFactory2.Object });

            contextFactory.Dispose();

            contextProviderFactory1.Verify(x => x.Dispose());
            contextProviderFactory2.Verify(x => x.Dispose());
        }

        [Test]
        public void It_should_create_context_with_all_context_provider_factories()
        {
            var contextProvider = new Mock<IContextProvider>();
            var contextProviderFactory1 = new Mock<IContextProviderFactory>();
            contextProviderFactory1.Setup(x => x.CreateContextProvider(null)).Returns(contextProvider.Object);
            var contextProviderFactory2 = new Mock<IContextProviderFactory>();
            contextProviderFactory2.Setup(x => x.CreateContextProvider(null)).Returns(contextProvider.Object);

            using (var contextFactory = new ContextFactory(new[] { contextProviderFactory1.Object, contextProviderFactory2.Object }))
            {
                contextFactory.Start();
                var context = contextFactory.StartNewContext();

                contextProviderFactory1.Verify(x => x.CreateContextProvider(It.IsAny<IContext>()));
                contextProviderFactory2.Verify(x => x.CreateContextProvider(It.IsAny<IContext>()));

                context.ContextFactory.Should().Be(contextFactory);
            }
        }

        [Test]
        public void It_should_not_create_context_if_not_ready()
        {
            var contextFactory = new ContextFactory(new IContextProviderFactory[0]);

            contextFactory.Invoking(x => x.StartNewContext())
                          .ShouldThrow<SlarshException>()
                          .WithMessage("started", ComparisonMode.Substring);
        }

    }
}

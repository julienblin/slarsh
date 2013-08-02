namespace Slarsh
{
    using System;

    public interface IContextProviderFactory : IValidatable, IDisposable
    {
        /// <summary>
        /// Starts the context provider. Will be called before any action on it.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        void Start(IContextFactory contextFactory);

        IContextProvider CreateContextProvider();
    }
}
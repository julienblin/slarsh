namespace Slarsh.Web
{
    /// <summary>
    /// The <see cref="ISlarshWebApplication"/> interface.
    /// A <see cref="System.Web.HttpApplication"/> that uses the <see cref="ContextPerRequestHttpModule"/>
    /// must implement this interface.
    /// </summary>
    public interface ISlarshWebApplication
    {
        /// <summary>
        /// Gets the <see cref="ContextFactoryConfiguration"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ContextFactoryConfiguration"/>.
        /// </returns>
        ContextFactoryConfiguration GetContextFactoryConfiguration();
    }
}

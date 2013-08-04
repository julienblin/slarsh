namespace Slarsh.Web.Mvc
{
    using System.Web.Mvc;

    /// <summary>
    /// Base <c>Slarsh</c> MVC controller.
    /// </summary>
    public abstract class SlarshController : Controller
    {
        /// <summary>
        /// Gets the current context.
        /// </summary>
        public IContext Context { get; internal set; }
    }
}

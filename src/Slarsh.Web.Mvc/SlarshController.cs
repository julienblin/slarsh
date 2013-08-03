namespace Slarsh.Web.Mvc
{
    using System.Web.Mvc;

    public abstract class SlarshController : Controller
    {
        public IContext Context { get; internal set; }
    }
}

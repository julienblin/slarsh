namespace SampleApp.Web.Controllers
{
    using System.Web.Mvc;

    using SampleApp.Core.Entities;
    using SampleApp.Core.Queries;

    using Slarsh;
    using Slarsh.Web.Mvc;

    public class HomeController : SlarshController
    {
        public ActionResult Index(EmployeeQuery query, PaginationParams pagination)
        {
            var result = Context.Fulfill(query).Paginate(pagination);
            return this.View(result);
        }
    }
}

namespace Slarsh.Web.Mvc
{
    using System.Web.Mvc;

    public class CurrentContextControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, System.Type controllerType)
        {
            var controller = base.GetControllerInstance(requestContext, controllerType);

            var slarshController = controller as SlarshController;
            if (slarshController != null)
            {
                slarshController.Context = Context.Current;
            }

            return controller;
        }
    }
}

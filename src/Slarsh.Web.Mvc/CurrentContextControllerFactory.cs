namespace Slarsh.Web.Mvc
{
    using System.Web.Mvc;

    /// <summary>
    /// <see cref="IControllerFactory"/> that injects the current <see cref="IContext"/> into <see cref="SlarshController.Context"/>.
    /// </summary>
    public class CurrentContextControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// Retrieves the controller instance for the specified request context and controller type.
        /// </summary>
        /// <returns>
        /// The controller instance.
        /// </returns>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param><param name="controllerType">The type of the controller.</param><exception cref="T:System.Web.HttpException"><paramref name="controllerType"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="controllerType"/> cannot be assigned.</exception><exception cref="T:System.InvalidOperationException">An instance of <paramref name="controllerType"/> cannot be created.</exception>
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

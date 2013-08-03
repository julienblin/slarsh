namespace SampleApp.Web
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    using SampleApp.Core.Entities;

    using Slarsh;
    using Slarsh.NHibernate;
    using Slarsh.Web;
    using Slarsh.Web.Mvc;

    public class MvcApplication : System.Web.HttpApplication, ISlarshWebApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new CurrentContextControllerFactory());
        }

        public ContextFactoryConfiguration GetContextFactoryConfiguration()
        {
            return new ContextFactoryConfiguration(
                new NHContextProviderFactory(
                    new NHContextProviderFactoryConfiguration
                    {
                        ConnectionString = ConfigurationManager.ConnectionStrings["sampleapp"].ConnectionString,
                        DatabaseType = DatabaseType.SqLite,
                        AutoUpdateSchemaOnStart = true,
                        MappingAssemblies = new[] { typeof(Employee).Assembly }
                    }))
                    {
                        CurrentContextHolder = new WebCurrentContextHolder()
                    };
        }
    }
}
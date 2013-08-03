namespace Slarsh.Web
{
    using System;
    using System.Web;

    /// <summary>
    /// An <see cref="IHttpModule"/> that will start Context factories and binds context on a per-HttpRequest basis.
    /// The <see cref="HttpApplication"/> (<c>Global.asax</c>) must implement the <see cref="ISlarshWebApplication"/> interface.
    /// </summary>
    public class ContextPerRequestHttpModule : IHttpModule
    {
        /// <summary>
        /// The application.
        /// </summary>
        private HttpApplication application;

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="app">
        /// The application.
        /// </param>
        public void Init(HttpApplication app)
        {
            this.application = app;
            if (!ContextFactory.HasCurrent)
            {
                var slarshApplication = app as ISlarshWebApplication;
                if (slarshApplication == null)
                {
                    throw new SlarshException(Resources.HttpApplicationMustImplementISlarshApplication);
                }

                ContextFactory.Start(slarshApplication.GetContextFactoryConfiguration());
            }
            
            this.application.BeginRequest += OnBeginRequest;
            this.application.EndRequest += OnEndRequest;
            this.application.Error += OnError;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            this.application.BeginRequest -= OnBeginRequest;
            this.application.EndRequest -= OnEndRequest;
            this.application.Error -= OnError;

            if (ContextFactory.HasCurrent)
            {
                ContextFactory.Current.Dispose();
            }
        }

        /// <summary>
        /// Starts a new context.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private static void OnBeginRequest(object sender, EventArgs eventArgs)
        {
            ContextFactory.StartNewContext();
        }

        /// <summary>
        /// Commit and dispose the current context if any.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private static void OnEndRequest(object sender, EventArgs eventArgs)
        {
            if (Context.HasCurrent)
            {
                Context.Current.Commit();
                Context.Current.Dispose();
                Context.Current = null;
            }
        }

        /// <summary>
        /// Dispose the current context if any, without committing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private static void OnError(object sender, EventArgs eventArgs)
        {
            if (Context.HasCurrent)
            {
                // We do not commit.
                Context.Current.Dispose();
                Context.Current = null;
            }
        }
    }
}

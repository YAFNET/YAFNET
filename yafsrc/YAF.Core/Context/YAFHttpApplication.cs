namespace YAF.Core.Context
{
    using System;
    using System.Web;
    using System.Web.Http;

    using YAF.Core.Context.Start;

    /// <summary>
    /// The YAF HttpApplication.
    /// </summary>
    public abstract class YafHttpApplication : HttpApplication
    {
        /// <summary>
        /// The application_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            // Pass a delegate to the Configure method.
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
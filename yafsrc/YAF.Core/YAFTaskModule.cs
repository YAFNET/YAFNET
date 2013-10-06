/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core
{
  #region Using

  using System;
  using System.Web;
  using System.Web.UI;

  using Autofac;

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.EventProxies;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Lifecycle module used to throw events around...
  /// </summary>
  public class YafTaskModule : IHttpModule, IHaveServiceLocator
  {
    #region Constants and Fields

    /// <summary>
    ///   The _app instance.
    /// </summary>
    protected HttpApplication _appInstance;

    /// <summary>
    ///   The _module initialized.
    /// </summary>
    protected bool _moduleInitialized;

    /// <summary>
    ///   Gets or sets the logger associated with the object.
    /// </summary>
    [Inject]
    public ILogger Logger { get; set; }

    [Inject]
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IHttpModule

    /// <summary>
    /// Bootstrapping fun
    /// </summary>
    /// <param name="httpApplication">
    /// The http application.
    /// </param>
    public void Init([NotNull] HttpApplication httpApplication)
    {
      CodeContracts.VerifyNotNull(httpApplication, "httpApplication");

      if (_moduleInitialized)
      {
        return;
      }

      // create a lock so no other instance can affect the static variable
      lock (this)
      {
        if (!_moduleInitialized)
        {
          _appInstance = httpApplication;

          // set the httpApplication as early as possible...
          GlobalContainer.Container.Resolve<CurrentHttpApplicationStateBaseProvider>().Instance =
            new HttpApplicationStateWrapper(httpApplication.Application);

          GlobalContainer.Container.Resolve<IInjectServices>().Inject(this);

          _moduleInitialized = true;

          _appInstance.PreRequestHandlerExecute += this.ApplicationPreRequestHandlerExecute;
        }
      }

      // app init notification...
      this.Get<IRaiseEvent>().RaiseIssolated(new HttpApplicationInitEvent(_appInstance), null);
    }

    /// <summary>
    /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
    /// </summary>
    void IHttpModule.Dispose()
    {
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The application pre request handler execute.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ApplicationPreRequestHandlerExecute([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (HttpContext.Current.CurrentHandler != null && HttpContext.Current.CurrentHandler is Page)
      {
        var page = HttpContext.Current.CurrentHandler as Page;

        try
        {
          // call from YafContext only -- so that the events have access to the full YafContext lifecycle.
          YafContext.Current.Get<IRaiseEvent>().RaiseIssolated(
            new EventPreRequestPageExecute(page),
            (m, ex) => this.Logger.Fatal(ex, "Failed to Call Event Pre Request Page Execute Event {0}".FormatWith(m)));
        }
        catch (Exception ex)
        {
          this.Logger.Fatal(ex, "Exception in PreRequestHandlerExecute.");
        }
      }
    }

    #endregion
  }
}
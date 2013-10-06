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
namespace YAF.Core.Services.Startup
{
  #region Using

    using System;
    using System.Web.UI;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The page request load services.
  /// </summary>
  [ExportService(ServiceLifetimeScope.OwnedByContainer)]
  public class PageRequestLoadServices : IHandleEvent<EventPreRequestPageExecute>, IHaveServiceLocator
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRequestLoadServices"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public PageRequestLoadServices([NotNull] IServiceLocator serviceLocator)
    {
      CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");

      this.ServiceLocator = serviceLocator;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order
    {
      get
      {
        return 10;
      }
    }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets or sets RequestPage.
    /// </summary>
    protected Page RequestPage { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<EventPreRequestPageExecute>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] EventPreRequestPageExecute @event)
    {
      this.RequestPage = @event.RequestedPage;
      this.RequestPage.PreInit += this.RequestedPage_PreInit;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The requested page_ pre init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    private void RequestedPage_PreInit([NotNull] object sender, [NotNull] EventArgs e)
    {
      // only run startup services for pages that require it.
      if (this.RequestPage.HasInterface<IRequireStartupServices>())
      {
        // run startup services...
        this.RunStartupServices();
      }
    }

    #endregion
  }
}
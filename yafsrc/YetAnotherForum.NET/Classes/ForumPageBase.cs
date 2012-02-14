/* YetAnotherForum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF
{
  #region Using

  using System;
  using System.Web;
  using System.Web.UI;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Optional forum page base providing some helper functions.
  /// </summary>
  public class ForumPageBase : Page, IHaveServiceLocator, IRequireStartupServices
  {
    #region Properties

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator
    {
      get
      {
        return YafContext.Current.ServiceLocator;
      }
    }

    public YafContext PageContext
    {
      get
      {
        return YafContext.Current;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The page_ error.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void Page_Error([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.Get<StartupInitializeDb>().Initialized)
      {
        return;
      }

      var error = this.Get<HttpServerUtilityBase>().GetLastError();

    	this.Get<ILoggerProvider>().Create(this.GetType()).Fatal(error.InnerException, "PageError: {0}", error.Message);
    }

    #endregion
  }
}
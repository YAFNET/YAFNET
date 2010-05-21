/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Pages
{
  // YAF.Pages
  using System;
  using System.Web.Security;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for logout.
  /// </summary>
  public partial class logout : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="logout"/> class.
    /// </summary>
    public logout()
      : base("LOGOUT")
    {
      PageContext.Globals.IsSuspendCheckEnabled = false;
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      FormsAuthentication.SignOut();
      // Clearing user cache with permissions data and active users cache...
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(String.Format(Constants.Cache.ActiveUserLazyData, this.PageContext.PageUserID)));
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.UsersOnlineStatus));
      
      Session.Abandon();      

      YafBuildLink.Redirect(ForumPages.forum);
    }

    #region Web Form Designer generated code

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    #endregion
  }
}
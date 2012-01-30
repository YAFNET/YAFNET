/* Yet Another Forum.NET
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
namespace YAF.Pages
{
  #region Using

  using System;
  using System.Web;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for _default.
  /// </summary>
  public partial class forum : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "forum" /> class.
    /// </summary>
    public forum()
      : base("DEFAULT")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.PollList.Visible = this.PageContext.BoardSettings.BoardPollID > 0;
      this.PollList.PollGroupId = this.PageContext.BoardSettings.BoardPollID;
      this.PollList.BoardId = this.PageContext.Settings.BoardID;

      if (!this.IsPostBack)
      {
        // vzrus: needs testing, potentially can cause problems 

        // Jaben: I can't access MY OWN FORUM with this code. Commented out. Either it's an optional feature or will be completely removed.
        // As far as I can see this is the worst kind of "feature": that no one asked for and solves a problem that no one had.

        //string ua = HttpContext.Current.Request.UserAgent;

        //if (!UserAgentHelper.IsSearchEngineSpider(ua) && (!UserAgentHelper.IsNotCheckedForCookiesAndJs(ua)))
        //{
        //  if (!HttpContext.Current.Request.Browser.Cookies)
        //  {
        //    YafBuildLink.RedirectInfoPage(InfoMessage.RequiresCookies);
        //  }

        //  Version ecmaVersion = HttpContext.Current.Request.Browser.EcmaScriptVersion;

        //  if (ecmaVersion != null)
        //  {
        //    try
        //    {
        //      string[] arrJsVer = Config.BrowserJSVersion.Split('.');

        //      if (!(ecmaVersion.Major >= arrJsVer[0].ToType<int>()) && !(ecmaVersion.Minor >= arrJsVer[1].ToType<int>()))
        //      {
        //        YafBuildLink.RedirectInfoPage(InfoMessage.EcmaScriptVersionUnsupported);
        //      }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //  }
        //  else
        //  {
        //    YafBuildLink.RedirectInfoPage(InfoMessage.RequiresEcmaScript);
        //  }
        //}

        this.ShoutBox1.Visible = this.PageContext.BoardSettings.ShowShoutbox;
        this.ForumStats.Visible = this.PageContext.BoardSettings.ShowForumStatistics;
        this.ActiveDiscussions.Visible = this.PageContext.BoardSettings.ShowActiveDiscussions;

        if (this.PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          if (this.PageContext.PageCategoryID != 0)
          {
            this.PageLinks.AddLink(
              this.PageContext.PageCategoryName, 
              YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
            this.Welcome.Visible = false;
          }
        }
      }
    }

    #endregion
  }
}
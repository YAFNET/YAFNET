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
namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The Main Forum Page.
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
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.PollList.Visible = this.Get<YafBoardSettings>().BoardPollID > 0;
      this.PollList.PollGroupId = this.Get<YafBoardSettings>().BoardPollID;
      this.PollList.BoardId = this.PageContext.Settings.BoardID;

      // Since these controls have EnabledViewState=false, set their visibility on every page load so that this value is not lost on postback.
      // This is important for another reason: these are board settings; values in the view state should have no impact on whether these controls are shown or not.
      this.ShoutBox1.Visible = this.Get<YafBoardSettings>().ShowShoutbox;
      this.ForumStats.Visible = this.Get<YafBoardSettings>().ShowForumStatistics;
      this.ActiveDiscussions.Visible = this.Get<YafBoardSettings>().ShowActiveDiscussions;

        if (this.IsPostBack)
        {
            return;
        }
        
        /*
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
        //}*/

        if (this.PageContext.Settings.LockedForum == 0)
        {
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            if (this.PageContext.PageCategoryID != 0)
            {
                this.PageLinks.AddLink(
                    this.PageContext.PageCategoryName, 
                    YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                this.Welcome.Visible = false;
            }
        }
    }

    #endregion
  }
}
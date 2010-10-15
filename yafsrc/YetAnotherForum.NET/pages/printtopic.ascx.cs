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
  using System.Data;
  using System.Linq;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for printtopic.
  /// </summary>
  public partial class printtopic : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="printtopic"/> class.
    /// </summary>
    public printtopic()
      : base("PRINTTOPIC")
    {
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
      if (Request.QueryString.GetFirstOrDefault("t") == null || !PageContext.ForumReadAccess)
      {
        YafBuildLink.AccessDenied();
      }

      ShowToolBar = false;

      if (!IsPostBack)
      {
        if (PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(PageContext.PageForumID);
        this.PageLinks.AddLink(PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", PageContext.PageTopicID));

        var dt = DB.post_list(
          PageContext.PageTopicID, 1, PageContext.BoardSettings.ShowDeletedMessages, false);

        // get max 500 rows
        var dataRows = dt.AsEnumerable().Take(500);

        // load the missing message test
        this.Get<YafDBBroker>().LoadMessageText(dataRows);

        this.Posts.DataSource = dataRows;

        DataBind();
      }
    }

    /// <summary>
    /// The get print header.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The get print header.
    /// </returns>
    protected string GetPrintHeader(object o)
    {
      var row = (DataRow) o;
      return "<strong>{2}: {0}</strong> - {1}".FormatWith(row["UserName"], this.Get<YafDateTime>().FormatDateTime((DateTime) row["Posted"]), this.GetText("postedby"));
    }

    /// <summary>
    /// The get print body.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The get print body.
    /// </returns>
    protected string GetPrintBody(object o)
    {
      var row = (DataRow) o;

      string message = row["Message"].ToString();

      message = YafFormatMessage.FormatMessage(message, new MessageFlags(Convert.ToInt32(row["Flags"])));

      return message;
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
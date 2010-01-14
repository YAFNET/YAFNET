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
namespace YAF.Pages.moderate
{
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;
  using YAF.Controls;

  /// <summary>
  /// Summary description for _default.
  /// </summary>
  public partial class reportedposts : ForumPage
  {
    #region Constructors & Overriden Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="reportedposts"/> class. 
    /// Default constructor.
    /// </summary>
    public reportedposts()
      : base("MODERATE_FORUM")
    {
    }


    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
      // forum index
      this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

      // moderation index
      this.PageLinks.AddLink(GetText("MODERATE_DEFAULT", "TITLE"), YafBuildLink.GetLink(ForumPages.moderate_index));

      // current page
      this.PageLinks.AddLink(PageContext.PageForumName);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // only forum moderators are allowed here
      if (!PageContext.IsModerator || !PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied();
      }

      // do this just on page load, not postbacks
      if (!IsPostBack)
      {
        // create page links
        CreatePageLinks();

        // bind data
        BindData();
      }
    }

    /// <summary>
    /// Handles load event for delete button, adds confirmation dialog.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load(object sender, EventArgs e)
    {
      var button = sender as ThemeButton;
      if (button != null)
      {
        button.Attributes["onclick"] = String.Format("return confirm('{0}');", GetText("ASK_DELETE"));
      }
    }

    /// <summary>
    /// Handles post moderation events/buttons.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void List_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
      // which command are we handling
      switch (e.CommandName.ToLower())
      {
        case "delete":

          // delete message
          DB.message_delete(e.CommandArgument, true, string.Empty, 1, true);

          // re-bind data
          BindData();

          // tell user message was deleted
          PageContext.AddLoadMessage(GetText("DELETED"));
          break;
        case "view":

          // go to the message
          YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", e.CommandArgument);
          break;
        case "copyover":

          // re-bind data
          BindData();

          // update message text
          DB.message_reportcopyover(e.CommandArgument);
          break;
        case "resolved":

          // mark message as resolved
          DB.message_reportresolve(9, e.CommandArgument, PageContext.PageUserID);

          // re-bind data
          BindData();

          // tell user message was flagged as resolved
          PageContext.AddLoadMessage(GetText("RESOLVEDFEEDBACK"));
          break;
      }

      // see if there are any items left...
      DataTable dt = DB.message_listreported(9, PageContext.PageForumID);

      if (dt.Rows.Count == 0)
      {
        // nope -- redirect back to the moderate main...
        YafBuildLink.Redirect(ForumPages.moderate_index);
      }
    }

    #endregion

    #region Data Binding & Formatting

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
      // get reported posts for this forum
      this.List.DataSource = DB.message_listreported(9, PageContext.PageForumID);

      // bind data to controls
      DataBind();
    }

    /// <summary>
    /// Format message.
    /// </summary>
    /// <param name="row">
    /// Message data row.
    /// </param>
    /// <returns>
    /// Formatted string with escaped HTML markup and formatted YafBBCode.
    /// </returns>
    protected string FormatMessage(DataRowView row)
    {
      // get message flags
      var messageFlags = new MessageFlags(row["Flags"]);

      // message
      string msg;

      // format message?
      if (messageFlags.NotFormatted)
      {
        // just encode it for HTML output
        msg = HtmlEncode(row["OriginalMessage"].ToString());
      }
      else
      {
        // fully format message (YafBBCode, smilies)
        msg = FormatMsg.FormatMessage(row["OriginalMessage"].ToString(), messageFlags, Convert.ToBoolean(row["IsModeratorChanged"]));
      }

      // return formatted message
      return msg;
    }

    #endregion

    #region Web Form Designer generated code

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      List.ItemCommand += new RepeaterCommandEventHandler(List_ItemCommand);

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
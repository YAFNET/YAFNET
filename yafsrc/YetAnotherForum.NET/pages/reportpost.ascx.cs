/* Yet Another Forum.NET
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
 * Written by vzrus (c) 2009 for Yet Another Forum.NET */

namespace YAF.Pages
{
  #region Using

  using System;
  using System.Data;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Editors;

  #endregion

  /// <summary>
  /// The form for reported post complaint text.
  /// </summary>
  public partial class ReportPost : ForumPage
  {
    // messageid
    #region Constants and Fields

    /// <summary>
    /// To save messageid value.
    /// </summary>
    private int messageID = 0;

    // message body editor

    /// <summary>
    /// The _editor.
    /// </summary>
    private BaseForumEditor reportEditor;

    #endregion

    //// Class constructor
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the ReportPost class.
    /// </summary>
    public ReportPost()
      : base("REPORTPOST")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The btn cancel query_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BtnCancel_Click(object sender, EventArgs e)
    {
      // Redirect to reported post
      this.RedirectToPost();
    }

    /// <summary>
    /// The btn run query_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BtnReport_Click(object sender, EventArgs e)
    {
      // Save the reported message
      DB.message_report(this.messageID, this.PageContext.PageUserID, DateTime.UtcNow, this.reportEditor.Text);
      
      // Redirect to reported post
      this.RedirectToPost();
    }

    //// public YAF.Editors.BaseForumEditor Editor { get {return _editor}; set; }

    /// <summary>
    /// Page initialization handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Init(object sender, EventArgs e)
    {
      // create editor based on administrator's settings
      this.reportEditor =
        YafContext.Current.EditorModuleManager.GetEditorInstance(YafContext.Current.BoardSettings.ForumEditor);

      // add editor to the page
      this.EditorLine.Controls.Add(this.reportEditor);
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
      // set attributes of editor
      this.reportEditor.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
      this.reportEditor.StyleSheet = YafContext.Current.Theme.BuildThemePath("theme.css");

      if (!String.IsNullOrEmpty(this.Request.QueryString["m"]))
      {
        // We check here if the user have access to the option
        if (!this.PageContext.BoardSettings.AllowGuestToReportPost && this.PageContext.IsGuest)
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1");
        }

        if (!Int32.TryParse(this.Request.QueryString["m"], out this.messageID))
        {
          YafBuildLink.Redirect(ForumPages.error, "Incorrect message value: {0}", this.messageID);
        }
      }

      if (!this.IsPostBack)
      {
        // Get reported message text for better quoting                    
        DataTable messageRow = DB.message_secdata(this.messageID, this.PageContext.PageUserID);

        // Checking if the user has a right to view the message and getting data  
        if (messageRow.Rows.Count > 0)
        {
          // populate the repeater with the message datarow...
          this.MessageList.DataSource = DB.message_secdata(this.messageID, this.PageContext.PageUserID);
          this.MessageList.DataBind();
        }
        else
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1");
        }

        // Get Forum Link
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.btnReport.Attributes.Add("onclick", String.Format("return confirm('{0}');", PageContext.Localization.GetText("CONFIRM_REPORTPOST")));
      }
    }

    /// <summary>
    /// Redirects to reported post after Save or Cancel
    /// </summary>
    protected void RedirectToPost()
    {
      // Redirect to reported post
      YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", this.messageID);
    }

    /// <summary>
    /// Binds data to data source
    /// </summary>
    private void BindData()
    {
      this.DataBind();
    }

    #endregion
  }
}
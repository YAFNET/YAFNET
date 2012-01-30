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
 * Written by vzrus (c) 2009 for Yet Another Forum.NET */

namespace YAF.Pages
{
  #region Using

  using System;
  using System.Data;
  using System.Web;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The form for reported post complaint text.
  /// </summary>
  public partial class ReportPost : ForumPage
  {
    // messageid
    #region Constants and Fields

    /// <summary>
    ///   To save messageid value.
    /// </summary>
    private int messageID;

    // message body editor

    /// <summary>
    ///   The _editor.
    /// </summary>
    private ForumEditor reportEditor;

    #endregion

    //// Class constructor
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the ReportPost class.
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
    protected void BtnCancel_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void BtnReport_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.reportEditor.Text.Length > this.Get<YafBoardSettings>().MaxReportPostChars)
      {
        this.IncorrectReportLabel.Text = this.GetTextFormatted(
          "REPORTTEXT_TOOLONG", this.Get<YafBoardSettings>().MaxReportPostChars);
        this.IncorrectReportLabel.DataBind();
        return;
      }

      // Save the reported message
      LegacyDb.message_report(this.messageID, this.PageContext.PageUserID, DateTime.UtcNow, this.reportEditor.Text);

      // Send Notification to Mods about the Reported Post.
      if (this.Get<YafBoardSettings>().EmailModeratorsOnReportedPost)
      {
          // not approved, notifiy moderators
          this.Get<ISendNotification>().ToModeratorsThatMessageWasReported(this.PageContext.PageForumID, this.messageID, this.PageContext.PageUserID, this.reportEditor.Text);
      }

      // Redirect to reported post
      this.RedirectToPost();
    }

    /// <summary>
    /// Page initialization handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Init([NotNull] object sender, [NotNull] EventArgs e)
    {
        // get the forum editor based on the settings
        string editorId = this.Get<YafBoardSettings>().ForumEditor;

        if (this.Get<YafBoardSettings>().AllowUsersTextEditor)
        {
            // Text editor
            editorId = !string.IsNullOrEmpty(this.PageContext.TextEditor)
                                    ? this.PageContext.TextEditor
                                    : this.Get<YafBoardSettings>().ForumEditor;
        }

        // Check if Editor exists, if not fallback to default editorid=1
        this.reportEditor =
           this.Get<IModuleManager<ForumEditor>>().GetBy(editorId, false) ??
           this.Get<IModuleManager<ForumEditor>>().GetBy("1");

        // Override Editor when mobile device with default Yaf BBCode Editor
        if (PageContext.IsMobileDevice)
        {
            this.reportEditor = this.Get<IModuleManager<ForumEditor>>().GetBy("1");
        }

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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // set attributes of editor
      this.reportEditor.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
      this.reportEditor.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");

      if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m").IsSet())
      {
        // We check here if the user have access to the option
        if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ReportPostPermissions))
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1");
        }

        if (!Int32.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), out this.messageID))
        {
          YafBuildLink.Redirect(ForumPages.error, "Incorrect message value: {0}", this.messageID);
        }
      }

        if (this.IsPostBack)
        {
            return;
        }

        // Get reported message text for better quoting                    
        DataTable messageRow = LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID);

        // Checking if the user has a right to view the message and getting data  
        if (messageRow.Rows.Count > 0)
        {
            // populate the repeater with the message datarow...
            this.MessageList.DataSource = LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID);
            this.MessageList.DataBind();
        }
        else
        {
            YafBuildLink.Redirect(ForumPages.info, "i=1");
        }

        // Get Forum Link
        this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
        this.btnReport.Attributes.Add(
            "onclick", "return confirm('{0}');".FormatWith(this.GetText("CONFIRM_REPORTPOST")));
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
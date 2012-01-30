/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Data;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for mail.
  /// </summary>
  public partial class mail : AdminPage
  {
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
        if (this.IsPostBack)
        {
            return;
        }

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_MAIL", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"),
              this.GetText("ADMIN_MAIL", "TITLE"));


        this.Send.Text = this.GetText("ADMIN_MAIL", "SEND_MAIL");
        this.Send.OnClientClick = "return confirm('{0}');".FormatWith(this.GetText("ADMIN_MAIL", "CONFIRM_SEND"));

        this.BindData();
    }

    /// <summary>
    /// The send_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Send_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      object groupID = null;

      if (this.ToList.SelectedItem.Value != "0")
      {
        groupID = this.ToList.SelectedValue;
      }

      string subject = this.Subject.Text.Trim();

      if (subject.IsNotSet())
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_MAIL", "MSG_SUBJECT"));
      }
      else
      {
        using (DataTable dt = LegacyDb.user_emails(this.PageContext.PageBoardID, groupID))
        {
          foreach (DataRow row in dt.Rows)
          {
            // Wes - Changed to use queue to improve scalability
            this.Get<ISendMail>().Queue(
              this.PageContext.BoardSettings.ForumEmail, 
              (string)row["Email"], 
              this.Subject.Text.Trim(), 
              this.Body.Text.Trim());
          }
        }

        this.Subject.Text = string.Empty;
        this.Body.Text = string.Empty;
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_MAIL", "MSG_QUEUED"));
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.ToList.DataSource = LegacyDb.group_list(this.PageContext.PageBoardID, null);
      this.DataBind();

      var item = new ListItem(this.GetText("ADMIN_MAIL", "ALL_USERS"), "0");
      this.ToList.Items.Insert(0, item);
    }

    #endregion
  }
}
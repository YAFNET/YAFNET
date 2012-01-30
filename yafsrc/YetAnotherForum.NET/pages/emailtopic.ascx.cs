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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Data;
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for emailtopic.
  /// </summary>
  public partial class emailtopic : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "emailtopic" /> class.
    /// </summary>
    public emailtopic()
      : base("EMAILTOPIC")
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
      if (this.Get<HttpRequestBase>().QueryString["t"] == null || !this.PageContext.ForumReadAccess ||
          !this.PageContext.BoardSettings.AllowEmailTopic)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
        if (this.PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(
            this.PageContext.PageCategoryName, 
            YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
        this.PageLinks.AddLink(
          this.PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));

        this.SendEmail.Text = this.GetText("send");

        this.Subject.Text = this.PageContext.PageTopicName;

        var emailTopic = new YafTemplateEmail();

        emailTopic.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(
          ForumPages.posts, true, "t={0}", this.PageContext.PageTopicID);
        emailTopic.TemplateParams["{user}"] = this.PageContext.PageUserName;

        this.Message.Text = emailTopic.ProcessTemplate("EMAILTOPIC");
      }
    }

    /// <summary>
    /// The send email_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SendEmail_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.EmailAddress.Text.Length == 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("need_email"));
        return;
      }

      try
      {
        string senderEmail = null;

        using (DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.PageContext.PageUserID, true))
        {
          senderEmail = (string)dt.Rows[0]["Email"];
        }

        // send the email...
        this.Get<ISendMail>().Send(
          senderEmail, this.EmailAddress.Text.Trim(), this.Subject.Text.Trim(), this.Message.Text.Trim());

        YafBuildLink.Redirect(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
      }
      catch (Exception x)
      {
        LegacyDb.eventlog_create(this.PageContext.PageUserID, this, x);
        this.PageContext.AddLoadMessage(this.GetTextFormatted("failed", x.Message));
      }
    }

    #endregion
  }
}
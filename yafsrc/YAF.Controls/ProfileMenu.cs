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
 */

namespace YAF.Controls
{
  #region Using

  using System.Text;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;

  #endregion

  /// <summary>
  /// Summary description for ForumUsers.
  /// </summary>
  public class ProfileMenu : BaseControl
  {
    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      var html = new StringBuilder(2000);

      html.Append(@"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafprofilemenu"">");

      if (this.PageContext.BoardSettings.AllowPrivateMessages)
      {
        html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText("MESSENGER"));
        html.AppendFormat(@"<tr><td class=""post""><ul id=""yafprofilemessenger"">");
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.cp_pm, "v=in"), 
          PageContext.Localization.GetText("INBOX"));
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.cp_pm, "v=out"), 
          PageContext.Localization.GetText("SENTITEMS"));
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.cp_pm, "v=arch"), 
          PageContext.Localization.GetText("ARCHIVE"));
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.pmessage), 
          PageContext.Localization.GetText("NEW_MESSAGE"));
        html.AppendFormat(@"</ul></td></tr>");
      }

      html.AppendFormat(
        @"<tr class=""header2""><td>{0}</td></tr>", this.PageContext.Localization.GetText("PERSONAL_PROFILE"));
      html.AppendFormat(@"<tr><td class=""post""><ul id=""yafprofilepersonal"">");
      html.AppendFormat(
        @"<li><a href=""{0}"">{1}</a></li>", 
        YafBuildLink.GetLink(ForumPages.profile, "u={0}", PageContext.PageUserID), 
        PageContext.Localization.GetText("VIEW_PROFILE"));
      html.AppendFormat(
        @"<li><a href=""{0}"">{1}</a></li>", 
        YafBuildLink.GetLink(ForumPages.cp_editprofile), 
        PageContext.Localization.GetText("EDIT_PROFILE"));
      if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableThanksMod)
      {
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.viewthanks, "u={0}", PageContext.PageUserID), 
          PageContext.Localization.GetText("ViewTHANKS", "TITLE"));
      }

      if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableBuddyList & this.PageContext.UserHasBuddies)
      {
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.cp_editbuddies), 
          PageContext.Localization.GetText("EDIT_BUDDIES"));
      }

      if (!this.PageContext.IsGuest && (this.PageContext.BoardSettings.EnableAlbum || (this.PageContext.NumAlbums > 0)))
      {
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID), 
          this.PageContext.Localization.GetText("EDIT_ALBUMS"));
      }

      html.AppendFormat(
        @"<li><a href=""{0}"">{1}</a></li>", 
        YafBuildLink.GetLink(ForumPages.cp_editavatar), 
        this.PageContext.Localization.GetText("EDIT_AVATAR"));
      if (this.PageContext.BoardSettings.AllowSignatures)
      {
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.cp_signature), 
          this.PageContext.Localization.GetText("SIGNATURE"));
      }

      html.AppendFormat(
        @"<li><a href=""{0}"">{1}</a></li>", 
        YafBuildLink.GetLink(ForumPages.cp_subscriptions), 
        this.PageContext.Localization.GetText("SUBSCRIPTIONS"));
      if (this.PageContext.BoardSettings.AllowPasswordChange)
      {
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", 
          YafBuildLink.GetLink(ForumPages.cp_changepassword), 
          this.PageContext.Localization.GetText("CHANGE_PASSWORD"));
      }

      html.AppendFormat(@"</ul></td></tr>");
      html.Append(@"</table>");

      writer.Write(html.ToString());
    }

    #endregion
  }
}
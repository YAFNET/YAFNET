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
using System.Text;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for ForumUsers.
  /// </summary>
  public class ProfileMenu : BaseControl
  {
    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      var html = new StringBuilder(2000);

      html.Append(@"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafprofilemenu"">");

      if (PageContext.BoardSettings.AllowPrivateMessages)
      {
        html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText("MESSENGER"));
        html.AppendFormat(@"<tr><td class=""post""><ul id=""yafprofilemessenger"">");
        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_pm, "v=in"), PageContext.Localization.GetText("INBOX"));
        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_pm, "v=out"), PageContext.Localization.GetText("SENTITEMS"));
        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_pm, "v=arch"), PageContext.Localization.GetText("ARCHIVE"));
        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.pmessage), PageContext.Localization.GetText("NEW_MESSAGE"));
        html.AppendFormat(@"</ul></td></tr>");
      }

      html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText("PERSONAL_PROFILE"));
      html.AppendFormat(@"<tr><td class=""post""><ul id=""yafprofilepersonal"">");
      html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_editprofile), PageContext.Localization.GetText("EDIT_PROFILE"));
      if (PageContext.BoardSettings.EnableBuddyList)
      {
          html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_editbuddies), PageContext.Localization.GetText("EDIT_BUDDIES"));
      }

      if (ShowAlbumsLink())
      {
          html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.albums, "u={0}", PageContext.PageUserID), PageContext.Localization.GetText("EDIT_ALBUMS"));
      }

      html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_editavatar), PageContext.Localization.GetText("EDIT_AVATAR"));
      if (PageContext.BoardSettings.AllowSignatures)
      {
        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_signature), PageContext.Localization.GetText("SIGNATURE"));
      }

      html.AppendFormat(
        @"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_subscriptions), PageContext.Localization.GetText("SUBSCRIPTIONS"));
      if (PageContext.BoardSettings.AllowPasswordChange)
      {
        html.AppendFormat(
          @"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.cp_changepassword), PageContext.Localization.GetText("CHANGE_PASSWORD"));
      }

      html.AppendFormat(@"</ul></td></tr>");
      html.Append(@"</table>");

      writer.Write(html.ToString());
    }
    private bool ShowAlbumsLink()
    {
        int albumUser = this.PageContext.PageUserID;       
        
        // Add check if Albums Tab is visible 
    
            int albumCount = YAF.Classes.Data.DB.album_getstats(albumUser, null)[0];

            // Check if the user already has albums.
            if (albumCount > 0)
            {
                return true;
            }
            else
            {
                // Check if a user have permissions to have albums, even if he has no albums at all.
                int? usrAlbums =
                  YAF.Classes.Data.DB.user_getalbumsdata(albumUser, this.PageContext.PageBoardID).GetFirstRowColumnAsValue<int?>(
                    "UsrAlbums", null);

                if (usrAlbums.HasValue)
                {
                    if (usrAlbums > 0 && this.PageContext.BoardSettings.EnableAlbum)
                    {
                        return true;
                    }
                }
            }
        

        return false;
       
    }
  }
}
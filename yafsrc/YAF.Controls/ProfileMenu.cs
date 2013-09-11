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

namespace YAF.Controls
{
    #region Using

    using System.Text;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// User Profile Menu in the User Control Panel
    /// </summary>
    public class ProfileMenu : BaseControl
    {
        #region Methods

        /// <summary>
        /// Render the Profile Menu
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            var html = new StringBuilder(2000);

            html.Append(@"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafprofilemenu"">");

            // Render Mailbox Items
            if (this.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", this.GetText("MESSENGER"));

                html.AppendFormat(@"<tr><td class=""post""><ul id=""yafprofilemessenger"">");

                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{3}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.cp_pm, "v=in"),
                    this.GetText("INBOX"),
                    ForumPages.cp_pm,
                    this.GetText("TOOLBAR", "INBOX_TITLE"));
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.cp_pm, "v=out"),
                    this.GetText("SENTITEMS"),
                    ForumPages.cp_pm);
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.cp_pm, "v=arch"),
                    this.GetText("ARCHIVE"), 
                    ForumPages.cp_pm);
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.pmessage),
                    this.GetText("NEW_MESSAGE"),
                    ForumPages.pmessage);

                html.AppendFormat(@"</ul></td></tr>");
            }

            // Render Personal Profile Items
            html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", this.GetText("PERSONAL_PROFILE"));
           
            html.AppendFormat(@"<tr><td class=""post""><ul id=""yafprofilepersonal"">");
            
            html.AppendFormat(
                @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                YafBuildLink.GetLink(ForumPages.profile, "u={0}", PageContext.PageUserID),
                this.GetText("VIEW_PROFILE"),
                    ForumPages.profile);
            html.AppendFormat(
                @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                YafBuildLink.GetLink(ForumPages.cp_editprofile),
                this.GetText("EDIT_PROFILE"),
                    ForumPages.cp_editprofile);

            if (!this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableThanksMod)
            {
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.viewthanks, "u={0}", PageContext.PageUserID),
                    this.GetText("ViewTHANKS", "TITLE"),
                    ForumPages.viewthanks);
            }

            if (!this.PageContext.IsGuest
                && this.Get<YafBoardSettings>().EnableBuddyList & this.PageContext.UserHasBuddies)
            {
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{3}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.cp_editbuddies),
                    this.GetText("EDIT_BUDDIES"),
                    ForumPages.cp_editbuddies,
                    this.GetText("TOOLBAR", "BUDDIES_TITLE"));
            }

            if (!this.PageContext.IsGuest
                && (this.Get<YafBoardSettings>().EnableAlbum || (this.PageContext.NumAlbums > 0)))
            {
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID),
                    this.GetText("EDIT_ALBUMS"),
                    ForumPages.albums);
            }

            if (!Config.IsDotNetNuke && (this.Get<YafBoardSettings>().AvatarRemote || this.Get<YafBoardSettings>().AvatarUpload
                || this.Get<YafBoardSettings>().AvatarGallery || this.Get<YafBoardSettings>().AvatarGravatar))
            {
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.cp_editavatar),
                    this.GetText("EDIT_AVATAR"),
                    ForumPages.cp_editavatar);
            }

            if (this.Get<YafBoardSettings>().AllowSignatures)
            {
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.cp_signature),
                    this.GetText("SIGNATURE"),
                    ForumPages.cp_signature);
            }

            html.AppendFormat(
                @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                YafBuildLink.GetLink(ForumPages.cp_subscriptions),
                this.GetText("SUBSCRIPTIONS"),
                    ForumPages.cp_subscriptions);

            if (!Config.IsDotNetNuke && this.Get<YafBoardSettings>().AllowPasswordChange)
            {
                html.AppendFormat(
                    @"<li class=""yafprofilemenu_{2}""><a href=""{0}"" title=""{1}"">{1}</a></li>",
                    YafBuildLink.GetLink(ForumPages.cp_changepassword),
                    this.GetText("CHANGE_PASSWORD"),
                    ForumPages.cp_changepassword);
            }

            html.AppendFormat(@"</ul></td></tr>");
            html.Append(@"</table>");

            writer.Write(html.ToString());
        }

        #endregion
    }
}
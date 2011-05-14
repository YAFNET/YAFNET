/* YetAnotherForum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Modules.BBCode
{
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// Hidden BBCode Module
    /// </summary>
    public class HideBBCodeModule : YafBBCodeControl
    {
        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            var hiddenContent = Parameters["inner"];

            var messageId = this.MessageID;

            if (hiddenContent.IsNotSet())
            {
                return;
            }

            var description = LocalizedString(
                "HIDDENMOD_DESC",
                "The content of this post is hidden. After you THANK the poster, refresh the page to see the hidden content. You only need to thank the Current Post");

            var descriptionGuest = LocalizedString(
                "HIDDENMOD_GUEST",
                "This board requires you to be registered and logged-in before you can view hidden messages.");

            string shownContentGuest =
                "<div class=\"ui-widget\"><div class=\"ui-state-error ui-corner-all HiddenGuestBox\"><p><span class=\"HiddenGuestBoxImage\"><img src=\"{1}\" alt=\"{0}\" title=\"{0}\" /></span>{2}</p><br /></div></div>"
                    .FormatWith(
                        description, YafForumInfo.GetURLToResource("images/HiddenWarnDescription.png"), descriptionGuest);

            string shownContent = "<img src=\"{1}\" alt=\"{0}\" title=\"{0}\" />".FormatWith(
                description, YafForumInfo.GetURLToResource("images/HiddenWarnDescription.png"));

            if (!this.Get<YafBoardSettings>().EnableThanksMod)
            {
                writer.Write(hiddenContent);
                return;
            }

            if (YafContext.Current.IsGuest)
            {
                writer.Write(shownContentGuest);
                return;
            }

            if (YafContext.Current.IsAdmin)
            {
                shownContent = hiddenContent;
            }
            else
            {
                var userId = YafContext.Current.CurrentUserData.UserID;

                if (DisplayUserID == userId ||
                        LegacyDb.user_ThankedMessage(messageId.ToType<int>(), userId))
                {
                    // Show hiddent content if user is the poster or have thanked the poster.
                    shownContent = hiddenContent;
                }
            }

            writer.Write(shownContent);
        }
    }
}
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
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlTypes;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The forum sub forum list.
    /// </summary>
    public partial class ForumSubForumList : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Sets DataSource.
        /// </summary>
        public IEnumerable DataSource
        {
            set
            {
                this.SubforumList.DataSource = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the "Forum Link Text" for the ForumList control.
        ///   Automatically disables the link if the current user doesn't
        ///   have proper permissions.
        /// </summary>
        /// <param name="row">
        /// Current data row
        /// </param>
        /// <returns>
        /// Forum link text
        /// </returns>
        public string GetForumLink([NotNull] DataRow row)
        {
            int forumID = row["ForumID"].ToType<int>();

            // get the Forum Description
            string output = Convert.ToString(row["Forum"]);

            if (int.Parse(row["ReadAccess"].ToString()) > 0)
            {
                output =
                    "<a href=\"{0}\" title=\"{1}\" >{2}</a>".FormatWith(
                        YafBuildLink.GetLink(ForumPages.topics, "f={0}", forumID),
                        this.GetText("COMMON", "VIEW_FORUM"),
                        output);
            }
            else
            {
                // no access to this forum
                output = "{0} {1}".FormatWith(output, this.GetText("NO_FORUM_ACCESS"));
            }

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The subforum list_ item created.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SubforumList_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var row = (DataRow)e.Item.DataItem;

            DateTime lastRead = this.Get<IReadTrackCurrentUser>().GetForumRead(
                row["ForumID"].ToType<int>(), row["LastForumAccess"].ToType<DateTime?>() ?? (DateTime)SqlDateTime.MinValue);

            DateTime lastPosted = row["LastPosted"].ToType<DateTime?>() ?? lastRead;

            var subForumIcon = e.Item.FindControl("ThemeSubforumIcon") as ThemeImage;

            if (subForumIcon == null)
            {
                return;
            }

            try
            {
                if (lastPosted > lastRead)
                {
                    subForumIcon.ThemeTag = "SUBFORUM_NEW";
                    subForumIcon.LocalizedTitlePage = "ICONLEGEND";
                    subForumIcon.LocalizedTitleTag = "NEW_POSTS";
                }
                else
                {
                    subForumIcon.ThemeTag = "SUBFORUM";
                    subForumIcon.LocalizedTitlePage = "ICONLEGEND";
                    subForumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
                }
            }
            catch
            {
                subForumIcon.ThemeTag = "SUBFORUM";
                subForumIcon.LocalizedTitlePage = "ICONLEGEND";
                subForumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
            }
        }

        #endregion
    }
}
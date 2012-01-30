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

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for printtopic.
    /// </summary>
    public partial class printtopic : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "printtopic" /> class.
        /// </summary>
        public printtopic()
            : base("PRINTTOPIC")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get print body.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The get print body.
        /// </returns>
        protected string GetPrintBody([NotNull] object o)
        {
            var row = (DataRow)o;

            string message = row["Message"].ToString();

            message = this.Get<IFormatMessage>().FormatMessage(message, new MessageFlags(row["Flags"].ToType<int>()));

            // Remove HIDDEN Text
            message = this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(message);

            message = this.Get<IFormatMessage>().RemoveCustomBBCodes(message);

            return message;
        }

        /// <summary>
        /// The get print header.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The get print header.
        /// </returns>
        protected string GetPrintHeader([NotNull] object o)
        {
            var row = (DataRow)o;
            string displayName = this.PageContext.Get<IUserDisplayName>().GetName((int)row["UserID"]);
            return "<strong>{2}: {0}</strong> - {1}".FormatWith(displayName.IsNotSet() ? row["UserName"] : displayName, this.Get<IDateTime>().FormatDateTime((DateTime)row["Posted"]), this.GetText("postedby"));
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
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
            if (this.Request.QueryString.GetFirstOrDefault("t") == null || !this.PageContext.ForumReadAccess)
            {
                YafBuildLink.AccessDenied();
            }

            this.ShowToolBar = false;

            if (this.IsPostBack)
            {
                return;
            }

            if (this.PageContext.Settings.LockedForum == 0)
            {
                this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(
                    this.PageContext.PageCategoryName,
                    YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
            }

            this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
            this.PageLinks.AddLink(
                this.PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));
            bool showDeleted = false;
            int userId = 0;
            if (this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }
            if (!showDeleted && ((this.Get<YafBoardSettings>().ShowDeletedMessages &&
                                  !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
                                 || this.PageContext.IsAdmin ||
                                 this.PageContext.IsForumModerator))
            {
                userId = this.PageContext.PageUserID;
            }

            var dt = LegacyDb.post_list(
                this.PageContext.PageTopicID,
                userId,
                1,
                showDeleted,
                false,
                DateTime.MinValue.AddYears(1901),
                DateTime.UtcNow,
                DateTime.MinValue.AddYears(1901),
                DateTime.UtcNow,
                0,
                500,
                2,
                0,
                0,
                false,
                -1);

            this.Posts.DataSource = dt.AsEnumerable();

            this.DataBind();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}
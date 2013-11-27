/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Topic Status Edit Page
    /// </summary>
    public partial class topicstatus_edit : AdminPage
    {
        #region Methods

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.save.Click += this.Add_Click;
            this.cancel.Click += this.Cancel_Click;

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
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_TOPICSTATUS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_topicstatus));
            this.PageLinks.AddLink(this.GetText("ADMIN_TOPICSTATUS_EDIT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_TOPICSTATUS", "TITLE"),
                this.GetText("ADMIN_TOPICSTATUS_EDIT", "TITLE"));

            this.save.Text = this.GetText("SAVE");
            this.cancel.Text = this.GetText("CANCEL");

            this.BindData();
        }

        /// <summary>
        /// The add_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Add_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (TopicStatusName.Text.Trim().IsNotSet() || DefaultDescription.Text.Trim().IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_TOPICSTATUS_EDIT", "MSG_ENTER"));

                this.BindData();
            }
            else
            {
                LegacyDb.TopicStatus_Save(
                    this.Request.QueryString.GetFirstOrDefault("i"),
                    this.PageContext.PageBoardID,
                    TopicStatusName.Text.Trim(),
                    DefaultDescription.Text.Trim());

                YafBuildLink.Redirect(ForumPages.admin_topicstatus);
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            if (!this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("i").IsSet())
            {
                return;
            }

            DataRow row =
                LegacyDb.TopicStatus_Edit(
                    Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("i"))).Rows[0];

            this.TopicStatusName.Text = (string)row["TopicStatusName"];
            this.DefaultDescription.Text = (string)row["DefaultDescription"];
        }

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_topicstatus);
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
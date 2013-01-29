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
    using System.Web;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for bannedip_edit.
    /// </summary>
    public partial class extensions_edit : AdminPage
    {
        #region Methods

        /// <summary>
        /// The is valid extension.
        /// </summary>
        /// <param name="newExtension">
        /// The new extension.
        /// </param>
        /// <returns>
        /// The is valid extension.
        /// </returns>
        protected bool IsValidExtension([NotNull] string newExtension)
        {
            if (newExtension.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EXTENSIONS_EDIT", "MSG_ENTER"));
                return false;
            }

            if (newExtension.IndexOf('.') != -1)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EXTENSIONS_EDIT", "MSG_REMOVE"));
                return false;
            }

            // TODO: maybe check for duplicate?
            return true;
        }

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
            //string strAddEdit = (this.Request.QueryString.GetFirstOrDefault("i") == null) ? "Add" : "Edit";

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_EXTENSIONS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_extensions));
            this.PageLinks.AddLink(this.GetText("ADMIN_EXTENSIONS_EDIT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_EXTENSIONS", "TITLE"),
                this.GetText("ADMIN_EXTENSIONS_EDIT", "TITLE"));

            this.save.Text = this.GetText("SAVE");
            this.cancel.Text = this.GetText("CANCEL");

            this.BindData();

            //this.extension.Attributes.Add("style", "width:250px");
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
            string ext = this.extension.Text.Trim();

            if (!this.IsValidExtension(ext))
            {
                this.BindData();
            }
            else
            {
                var entity = new FileExtension()
                                    {
                                        ID = this.Request.QueryString.GetFirstOrDefaultAs<int?>("i") ?? 0,
                                        BoardId = this.PageContext.PageBoardID,
                                        Extension = ext
                                    };

                this.GetRepository<FileExtension>().Upsert(entity);
                YafBuildLink.Redirect(ForumPages.admin_extensions);
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var extensionId = this.Request.QueryString.GetFirstOrDefaultAs<int?>("i");

            if (!extensionId.HasValue)
            {
                return;
            }

            var fileExtension = this.GetRepository<FileExtension>().GetByID(extensionId.Value);
            this.extension.Text = fileExtension.Extension;
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
            YafBuildLink.Redirect(ForumPages.admin_extensions);
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
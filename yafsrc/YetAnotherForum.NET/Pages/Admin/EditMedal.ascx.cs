/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    using Image = System.Drawing.Image;

    #endregion

    /// <summary>
    /// Administration Page to Add/Edit Medals
    /// </summary>
    public partial class EditMedal : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EditMedal" /> class. 
        ///   Default constructor.
        /// </summary>
        public EditMedal()
            : base("ADMIN_EDITMEDAL")
        {
        }

        #endregion

        /// <summary>
        /// Gets the current medal identifier.
        /// </summary>
        /// <value>
        /// The current medal identifier.
        /// </value>
        protected int? CurrentMedalId =>
            this.Get<HttpRequestBase>().QueryString.Exists("medalid")
                ? this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int?>("medalid")
                : null;

        #region Methods

        /// <summary>
        /// Handles click on add group button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Shows user-medal adding/editing controls.
        /// </remarks>
        protected void AddGroupClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GroupEditDialog.BindData(null, this.CurrentMedalId);

            BoardContext.Current.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("GroupEditDialog"));
        }

        /// <summary>
        /// Handles click on add user button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Shows user-medal adding/editing controls.
        /// </remarks>
        protected void AddUserClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UserEditDialog.BindData(null, this.CurrentMedalId);

            BoardContext.Current.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("UserEditDialog"));
        }

        /// <summary>
        /// Handles click on cancel button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // go back to medals administration
            BuildLink.Redirect(ForumPages.Admin_Medals);
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            // administration index
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(
                this.GetText("ADMIN_MEDALS", "TITLE"),
                BuildLink.GetLink(ForumPages.Admin_Medals));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITMEDAL", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_MEDALS", "TITLE")} - {this.GetText("ADMIN_EDITMEDAL", "TITLE")}";
        }

        /// <summary>
        /// Creates link to group editing admin interface.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The format group link.
        /// </returns>
        protected string FormatGroupLink([NotNull] object data)
        {
            var dr = (DataRowView)data;

            return string.Format(
                "<a href=\"{1}\">{0}</a>",
                dr["GroupName"],
                BuildLink.GetLink(ForumPages.Admin_EditGroup, "i={0}", dr["GroupID"]));
        }

        /// <summary>
        /// Creates link to user editing admin interface.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The format user link.
        /// </returns>
        protected string FormatUserLink([NotNull] object data)
        {
            var dr = (DataRowView)data;

            return string.Format(
                "<a href=\"{2}\">{0}&nbsp;({1})</a>",
                this.HtmlEncode(dr["DisplayName"]),
                this.HtmlEncode(dr["UserName"]),
                BuildLink.GetLink(ForumPages.Admin_EditUser, "u={0}", dr["UserID"]));
        }

        /// <summary>
        /// Handles click on GroupList repeaters item command link button.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void GroupListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    this.GroupEditDialog.BindData(e.CommandArgument.ToType<int>(), this.CurrentMedalId);

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("GroupEditDialog"));
                    break;
                case "remove":
                    this.GetRepository<GroupMedal>().Delete(
                        medal => medal.GroupID == e.CommandArgument.ToType<int>()
                                 && medal.MedalID == this.CurrentMedalId);

                    // remove all user medals...
                    this.RemoveMedalsFromCache();

                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // this needs to be done just once, not during post-backs
            if (!this.IsPostBack)
            {
                // bind data
                this.BindData();
            }

            // set previews
            this.SetPreview(this.MedalImage, this.MedalPreview);
            this.SetPreview(this.RibbonImage, this.RibbonPreview);
            this.SetPreview(this.SmallMedalImage, this.SmallMedalPreview);
            this.SetPreview(this.SmallRibbonImage, this.SmallRibbonPreview);
        }

        /// <summary>
        /// Removals all medals from the cache...
        /// </summary>
        protected void RemoveMedalsFromCache()
        {
            // remove all user medals...
            this.Get<IDataCache>().Remove(k => k.StartsWith(string.Format(Constants.Cache.UserMedals, string.Empty)));
        }

        /// <summary>
        /// Handles save button click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.MedalImage.SelectedIndex <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITMEDAL", "MSG_IMAGE"), MessageTypes.warning);
                return;
            }

            if (this.SmallMedalImage.SelectedIndex <= 0)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITMEDAL", "MSG_SMALL_IMAGE"),
                    MessageTypes.warning);
                return;
            }

            if (this.SortOrder.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_VALUE"), MessageTypes.warning);
                return;
            }

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITFORUM", "MSG_POSITIVE_VALUE"),
                    MessageTypes.warning);
                return;
            }

            if (!byte.TryParse(this.SortOrder.Text.Trim(), out var sortOrder))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_CATEGORY"), MessageTypes.warning);
                return;
            }

            // data
            string ribbonUrl = null, smallRibbonUrl = null;
            short? ribbonWidth = null, ribbonHeight = null;
            Size imageSize;

            // flags
            var flags = new MedalFlags(0)
                            {
                                ShowMessage = this.ShowMessage.Checked,
                                AllowRibbon = this.AllowRibbon.Checked,
                                AllowReOrdering = this.AllowReOrdering.Checked,
                                AllowHiding = this.AllowHiding.Checked
                            };

            // get medal images
            var imageUrl = this.MedalImage.SelectedValue;
            var smallImageUrl = this.SmallMedalImage.SelectedValue;
            if (this.RibbonImage.SelectedIndex > 0)
            {
                ribbonUrl = this.RibbonImage.SelectedValue;
            }

            if (this.SmallRibbonImage.SelectedIndex > 0)
            {
                smallRibbonUrl = this.SmallRibbonImage.SelectedValue;

                imageSize = this.GetImageSize(smallRibbonUrl);
                ribbonWidth = imageSize.Width.ToType<short>();
                ribbonHeight = imageSize.Height.ToType<short>();
            }

            // get size of small image
            imageSize = this.GetImageSize(smallImageUrl);

            // save medal
            this.GetRepository<Medal>().Save(
                this.CurrentMedalId,
                this.Name.Text,
                this.Description.Text,
                this.Message.Text,
                this.Category.Text,
                imageUrl,
                ribbonUrl,
                smallImageUrl,
                smallRibbonUrl,
                (short)imageSize.Width,
                (short)imageSize.Height,
                ribbonWidth,
                ribbonHeight,
                sortOrder,
                flags.BitValue);

            // go back to medals administration
            BuildLink.Redirect(ForumPages.Admin_Medals);
        }

        /// <summary>
        /// Handles click on UserList repeaters item command link button.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void UserListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    this.UserEditDialog.BindData(e.CommandArgument.ToType<int>(), this.CurrentMedalId);

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("UserEditDialog"));
                    break;
                case "remove":

                    // delete user-medal
                    this.GetRepository<UserMedal>().Delete(
                        medal => medal.UserID == e.CommandArgument.ToType<int>()
                                 && medal.MedalID == this.CurrentMedalId);

                    // clear cache...
                    this.Get<IDataCache>().Remove(
                        string.Format(Constants.Cache.UserMedals, e.CommandArgument.ToType<int>()));
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // load available images from images/medals folder
            using (var dt = new DataTable("Files"))
            {
                // create structure
                dt.Columns.Add("FileID", typeof(long));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("Description", typeof(string));

                // add blank row
                var dr = dt.NewRow();
                dr["FileID"] = 0;
                dr["FileName"] =
                    BoardInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
                dr["Description"] = this.GetText("ADMIN_EDITMEDAL", "SELECT_IMAGE");
                dt.Rows.Add(dr);

                // add files from medals folder
                var dir = new DirectoryInfo(
                    this.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}{BoardFolders.Current.Medals}"));
                var files = dir.GetFiles("*.*");

                long fileId = 1;

                foreach (var file in from file in files
                                     let sExt = file.Extension.ToLower()
                                     where sExt == ".png" || sExt == ".gif" || sExt == ".jpg"
                                     select file)
                {
                    dr = dt.NewRow();
                    dr["FileID"] = fileId++;
                    dr["FileName"] = file.Name;
                    dr["Description"] = file.Name;
                    dt.Rows.Add(dr);
                }

                // medal image
                this.MedalImage.DataSource = dt;
                this.MedalImage.DataValueField = "FileName";
                this.MedalImage.DataTextField = "Description";

                // ribbon bar image
                this.RibbonImage.DataSource = dt;
                this.RibbonImage.DataValueField = "FileName";
                this.RibbonImage.DataTextField = "Description";

                // small medal image
                this.SmallMedalImage.DataSource = dt;
                this.SmallMedalImage.DataValueField = "FileName";
                this.SmallMedalImage.DataTextField = "Description";

                // small ribbon bar image
                this.SmallRibbonImage.DataSource = dt;
                this.SmallRibbonImage.DataValueField = "FileName";
                this.SmallRibbonImage.DataTextField = "Description";
            }

            // bind data to controls
            this.DataBind();

            // load existing medal if we are editing one
            if (this.CurrentMedalId.HasValue)
            {
                // load users and groups who has been assigned this medal
                this.UserList.DataSource = this.GetRepository<UserMedal>().ListAsDataTable(null, this.CurrentMedalId);
                this.UserList.DataBind();

                this.GroupList.DataSource =
                    this.GetRepository<Medal>().GroupMedalListAsDataTable(null, this.CurrentMedalId);
                this.GroupList.DataBind();

                var medal = this.GetRepository<Medal>().GetSingle(m => m.ID == this.CurrentMedalId);

                // set controls
                this.Name.Text = medal.Name;
                this.Description.Text = medal.Description;
                this.Message.Text = medal.Message;
                this.Category.Text = medal.Category;
                this.SortOrder.Text = medal.SortOrder.ToString();
                this.ShowMessage.Checked = medal.MedalFlags.ShowMessage;
                this.AllowRibbon.Checked = medal.MedalFlags.AllowRibbon;
                this.AllowHiding.Checked = medal.MedalFlags.AllowHiding;
                this.AllowReOrdering.Checked = medal.MedalFlags.AllowReOrdering;

                // select images
                this.SelectImage(this.MedalImage, this.MedalPreview, medal.MedalURL);
                this.SelectImage(this.RibbonImage, this.RibbonPreview, medal.RibbonURL);
                this.SelectImage(this.SmallMedalImage, this.SmallMedalPreview, medal.SmallMedalURL);
                this.SelectImage(this.SmallRibbonImage, this.SmallRibbonPreview, medal.SmallRibbonURL);
            }
            else
            {
                // set all previews on blank image
                var spacerPath =
                    BoardInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry

                this.MedalPreview.Src = spacerPath;
                this.RibbonPreview.Src = spacerPath;
                this.SmallMedalPreview.Src = spacerPath;
                this.SmallRibbonPreview.Src = spacerPath;
            }
        }

        /// <summary>
        /// Gets size of image located in medals directory.
        /// </summary>
        /// <param name="filename">
        /// Name of file.
        /// </param>
        /// <returns>
        /// Size of image.
        /// </returns>
        private Size GetImageSize([NotNull] string filename)
        {
            using (var img = Image.FromFile(
                this.Server.MapPath($"{BoardInfo.ForumServerFileRoot}{BoardFolders.Current.Medals}/{filename}")))
            {
                return img.Size;
            }
        }

        /// <summary>
        /// Select image in dropdown list and sets appropriate preview.
        /// </summary>
        /// <param name="list">
        /// DropDownList where to search.
        /// </param>
        /// <param name="preview">
        /// Preview image.
        /// </param>
        /// <param name="imageUrl">
        /// URL to search for.
        /// </param>
        private void SelectImage([NotNull] DropDownList list, [NotNull] HtmlImage preview, [NotNull] string imageUrl)
        {
            // try to find item in a list
            var item = list.Items.FindByText(imageUrl);

            if (item != null)
            {
                // select found item
                item.Selected = true;

                // set preview image
                preview.Src = $"{BoardInfo.ForumClientFileRoot}{BoardFolders.Current.Medals}/{imageUrl}";
            }
            else
            {
                // if we found nothing, set blank image as preview
                preview.Src = BoardInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
            }
        }

        /// <summary>
        /// Sets the Image preview.
        /// </summary>
        /// <param name="imageSelector">DropDownList with image file listed.</param>
        /// <param name="imagePreview">Image for showing preview.</param>
        private void SetPreview([NotNull] WebControl imageSelector, [NotNull] HtmlControl imagePreview)
        {
            // create javascript
            imageSelector.Attributes["onchange"] =
                $"getElementById('{imagePreview.ClientID}').src='{BoardInfo.ForumClientFileRoot}{BoardFolders.Current.Medals}/' + this.value";
        }

        #endregion
    }
}
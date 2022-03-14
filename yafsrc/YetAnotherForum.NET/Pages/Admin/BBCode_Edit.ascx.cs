/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The BBCode Admin Edit Page.
    /// </summary>
    public partial class BBCode_Edit : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCode_Edit"/> class. 
        /// </summary>
        public BBCode_Edit()
            : base("ADMIN_BBCODE_EDIT", ForumPages.Admin_BBCode_Edit)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The bb code id.
        /// </summary>
        public int? BBCodeID =>
            this.Get<HttpRequestBase>().QueryString.Exists("b")
                ? this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("b")
                : null;

        #endregion

        #region Methods

        /// <summary>
        /// Adds the New BB Code or saves the existing one
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Add_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!ValidationHelper.IsValidPosShort(this.txtExecOrder.Text.Trim()))
            {
                this.PageBoardContext.AddLoadMessage(
                    this.GetText("ADMIN_BBCODE_EDIT", "MSG_POSITIVE_VALUE"),
                    MessageTypes.warning);
                return;
            }

            this.GetRepository<Types.Models.BBCode>().Save(
                this.BBCodeID,
                this.txtName.Text.Trim(),
                this.txtDescription.Text,
                this.txtOnClickJS.Text,
                this.txtDisplayJS.Text,
                this.txtEditJS.Text,
                this.txtDisplayCSS.Text,
                this.txtSearchRegEx.Text,
                this.txtReplaceRegEx.Text,
                this.txtVariables.Text,
                this.chkUseModule.Checked,
                this.UseToolbar.Checked,
                this.txtModuleClass.Text,
                this.txtExecOrder.Text.ToType<short>());

            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_BBCodes);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        protected void BindData()
        {
            if (!this.BBCodeID.HasValue)
            {
                return;
            }

            var code = this.GetRepository<Types.Models.BBCode>().GetById(this.BBCodeID.Value);

            // fill the control values...
            this.txtName.Text = code.Name;
            this.txtExecOrder.Text = code.ExecOrder.ToString();
            this.txtDescription.Text = code.Description;
            this.txtOnClickJS.Text = code.OnClickJS;
            this.txtDisplayJS.Text = code.DisplayJS;
            this.txtEditJS.Text = code.EditJS;
            this.txtDisplayCSS.Text = code.DisplayCSS;
            this.txtSearchRegEx.Text = code.SearchRegex;
            this.txtReplaceRegEx.Text = code.ReplaceRegex;
            this.txtVariables.Text = code.Variables;
            this.txtModuleClass.Text = code.ModuleClass;
            this.chkUseModule.Checked = code.UseModule ?? false;
            this.UseToolbar.Checked = code.UseToolbar ?? false;
        }

        /// <summary>
        /// Cancel Edit
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_BBCodes);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.FormValidatorJs),
                JavaScriptBlocks.FormValidatorJs(this.Save.ClientID));

            this.BindData();
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            var strAddEdit = this.BBCodeID == null ? this.GetText("COMMON", "ADD") : this.GetText("COMMON", "EDIT");

            this.PageLinks.AddRoot();
            this.PageLinks.AddAdminIndex();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_BBCODE", "TITLE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_BBCodes));
            this.PageLinks.AddLink(string.Format(this.GetText("ADMIN_BBCODE_EDIT", "TITLE"), strAddEdit), string.Empty);
        }

        #endregion
    }
}
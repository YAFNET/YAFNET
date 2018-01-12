/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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

    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The bbcode_edit.
    /// </summary>
    public partial class bbcode_edit : AdminPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _BBCode id.
        /// </summary>
        private int? _bbcodeId;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets BBCodeID.
        /// </summary>
        protected int? BBCodeID
        {
            get
            {
                if (this._bbcodeId != null)
                {
                    return this._bbcodeId;
                }

                if (this.Request.QueryString.GetFirstOrDefault("b") == null)
                {
                    return null;
                }

                int id;

                if (!int.TryParse(this.Request.QueryString.GetFirstOrDefault("b"), out id))
                {
                    return null;
                }

                this._bbcodeId = id;
                return id;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the New BB Code or saves the existing one
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Add_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            short sortOrder;

            if (!ValidationHelper.IsValidPosShort(this.txtExecOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_BBCODE_EDIT", "MSG_POSITIVE_VALUE"));
                return;
            }

            if (!short.TryParse(this.txtExecOrder.Text.Trim(), out sortOrder))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_BBCODE_EDIT", "MSG_NUMBER"));
                return;
            }

            this.GetRepository<BBCode>()
                .Save(
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
                    this.txtModuleClass.Text,
                    sortOrder);

            this.Get<IDataCache>().Remove(Constants.Cache.CustomBBCode);
            this.Get<IObjectStore>().RemoveOf<IProcessReplaceRules>();

            YafBuildLink.Redirect(ForumPages.admin_bbcode);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        protected void BindData()
        {
            if (this.BBCodeID == null)
            {
                return;
            }

            var bbCode = this.GetRepository<BBCode>().GetById(this.BBCodeID.Value);

            // fill the control values...
            this.txtName.Text = bbCode.Name;
            this.txtExecOrder.Text = bbCode.ExecOrder.ToString();
            this.txtDescription.Text = bbCode.Description;
            this.txtOnClickJS.Text = bbCode.OnClickJS;
            this.txtDisplayJS.Text = bbCode.DisplayJS;
            this.txtEditJS.Text = bbCode.EditJS;
            this.txtDisplayCSS.Text = bbCode.DisplayCSS;
            this.txtSearchRegEx.Text = bbCode.SearchRegex;
            this.txtReplaceRegEx.Text = bbCode.ReplaceRegex;
            this.txtVariables.Text = bbCode.Variables;
            this.txtModuleClass.Text = bbCode.ModuleClass;
            this.chkUseModule.Checked = bbCode.UseModule ?? false;
        }

        /// <summary>
        /// Cancel Edit
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_bbcode);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var strAddEdit = (this.BBCodeID == null) ? this.GetText("COMMON", "ADD") : this.GetText("COMMON", "EDIT");

            if (!this.IsPostBack)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.GetText("ADMIN_ADMIN", "Administration"),
                    YafBuildLink.GetLink(ForumPages.admin_admin));
                this.PageLinks.AddLink(
                    this.GetText("ADMIN_BBCODE", "TITLE"),
                    YafBuildLink.GetLink(ForumPages.admin_bbcode));
                this.PageLinks.AddLink(this.GetText("ADMIN_BBCODE_EDIT", "TITLE").FormatWith(strAddEdit), string.Empty);

                this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                    this.GetText("ADMIN_ADMIN", "Administration"),
                    this.GetText("ADMIN_BBCODE", "TITLE"),
                    this.GetText("ADMIN_BBCODE_EDIT", "TITLE").FormatWith(strAddEdit));
                this.BindData();
            }

            // TODO : Remove Hardcoded Styles, and move them to css
            this.txtName.Attributes.Add("style", "width:99%");

            const string Style = "width:99%;height:75px;";

            this.txtDescription.Attributes.Add("style", Style);
            this.txtOnClickJS.Attributes.Add("style", Style);
            this.txtDisplayJS.Attributes.Add("style", Style);
            this.txtEditJS.Attributes.Add("style", Style);
            this.txtDisplayCSS.Attributes.Add("style", Style);
            this.txtSearchRegEx.Attributes.Add("style", Style);
            this.txtReplaceRegEx.Attributes.Add("style", Style);
            this.txtVariables.Attributes.Add("style", Style);
            this.txtModuleClass.Attributes.Add("style", "width:99%");
        }

        #endregion
    }
}
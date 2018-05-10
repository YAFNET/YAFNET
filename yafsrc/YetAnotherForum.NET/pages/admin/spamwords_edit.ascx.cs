/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2017 Ingo Herbote
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
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin spam words edit page.
    /// </summary>
    public partial class spamwords_edit : AdminPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _SpamWord id.
        /// </summary>
        private int? _spamWordId;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets SpamWordID.
        /// </summary>
        protected int? SpamWordID
        {
            get
            {
                if (this._spamWordId != null)
                {
                    return this._spamWordId;
                }

                if (this.Request.QueryString.GetFirstOrDefault("i") == null)
                {
                    return null;
                }

                int id;

                if (!int.TryParse(this.Request.QueryString.GetFirstOrDefault("i"), out id))
                {
                    return null;
                }

                this._spamWordId = id;
                return id;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check if Valid Expression
        /// </summary>
        /// <param name="newExpression">
        /// The new Expression to Check.
        /// </param>
        /// <returns>
        /// Returns if Valid Expression
        /// </returns>
        protected bool IsValidWordExpression([NotNull] string newExpression)
        {
            if (!newExpression.Equals("*"))
            {
                return true;
            }

            this.PageContext.AddLoadMessage(this.GetText("ADMIN_SPAMWORDS_EDIT", "MSG_REGEX_SPAM"));
            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.save.Click += this.Add_Click;
            this.cancel.Click += this.Cancel_Click;

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.GetText("ADMIN_ADMIN", "Administration"),
                    YafBuildLink.GetLink(ForumPages.admin_admin));
                this.PageLinks.AddLink(
                    this.GetText("ADMIN_SPAMWORDS", "TITLE"),
                    YafBuildLink.GetLink(ForumPages.admin_spamwords));
                this.PageLinks.AddLink(this.GetText("ADMIN_SPAMWORDS_EDIT", "TITLE"), string.Empty);

                this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                    this.GetText("ADMIN_ADMIN", "Administration"),
                    this.GetText("ADMIN_SPAMWORDS", "TITLE"),
                    this.GetText("ADMIN_SPAMWORDS_EDIT", "TITLE"));

                this.save.Text = this.GetText("COMMON", "SAVE");
                this.cancel.Text = this.GetText("COMMON", "CANCEL");

                this.BindData();
            }

            this.spamword.Attributes.Add("style", "width:250px");
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            int id;

            if (this.Request.QueryString.GetFirstOrDefault("i") == null
                || !int.TryParse(this.Request.QueryString.GetFirstOrDefault("i"), out id))
            {
                return;
            }

            if (this.SpamWordID == null)
            {
                return;
            }

            var spamWord =
                this.GetRepository<Spam_Words>().ListTyped(this.SpamWordID.Value, this.PageContext.PageBoardID)[0];
            this.spamword.Text = spamWord.SpamWord;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Add_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsValidWordExpression(this.spamword.Text.Trim()))
            {
                this.BindData();
            }
            else
            {
                this.GetRepository<Spam_Words>()
                    .Save(
                        spamWordID: this.Request.QueryString.GetFirstOrDefaultAs<int>("i"),
                        spamWord: this.spamword.Text);

                this.Get<IDataCache>().Remove(Constants.Cache.SpamWords);
                YafBuildLink.Redirect(ForumPages.admin_spamwords);
            }
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_spamwords);
        }

        #endregion
    }
}
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

namespace YAF.Dialogs
{
    #region Using

    using System;

    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Replace Words Add/Edit Dialog.
    /// </summary>
    public partial class ReplaceWordsEdit : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the spam word identifier.
        /// </summary>
        /// <value>
        /// The spam word identifier.
        /// </value>
        public int? ReplaceWordId
        {
            get
            {
                return this.ViewState["ReplaceWordId"].ToType<int?>();
            }

            set
            {
                this.ViewState["ReplaceWordId"] = value;
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="replaceWordId">The replace word identifier.</param>
        public void BindData(int? replaceWordId)
        {
            this.ReplaceWordId = replaceWordId;

            this.Title.LocalizedPage = "ADMIN_REPLACEWORDS_EDIT";
            this.Save.TextLocalizedPage = "ADMIN_REPLACEWORDS";

            if (this.ReplaceWordId.HasValue)
            {
                // Edit
                var replaceWord = this.GetRepository<Replace_Words>().GetById(this.ReplaceWordId.Value);

                this.badword.Text = replaceWord.BadWord;
                this.goodword.Text = replaceWord.GoodWord;

                this.Title.LocalizedTag = "TITLE_EDIT";
                this.Save.TextLocalizedTag = "SAVE";
            }
            else
            {
                // Add
                this.badword.Text = string.Empty;
                this.goodword.Text = string.Empty;

                this.Title.LocalizedTag = "TITLE";
                this.Save.TextLocalizedTag = "ADD";
            }
        }

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

            this.PageContext.AddLoadMessage(this.GetText("ADMIN_REPLACEWORDS_EDIT", "MSG_REGEX_BAD"));
            return false;
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsValidWordExpression(this.badword.Text.Trim()))
            {
                return;
            }

            this.GetRepository<Replace_Words>()
                .Save(
                    replaceWordId: this.ReplaceWordId,
                    badWord: this.badword.Text,
                    goodWord: this.goodword.Text);

            this.Get<IDataCache>().Remove(Constants.Cache.ReplaceWords);

            YafBuildLink.Redirect(ForumPages.admin_replacewords);
        }

        #endregion
    }
}
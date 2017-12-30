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
    /// The Admin Spam Words Add/Edit Dialog.
    /// </summary>
    public partial class SpamWordsEdit : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the spam word identifier.
        /// </summary>
        /// <value>
        /// The spam word identifier.
        /// </value>
        public int? SpamWordId
        {
            get
            {
                return this.ViewState["SpamWordId"].ToType<int?>();
            }

            set
            {
                this.ViewState["SpamWordId"] = value;
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="spamWordId">The spam word identifier.</param>
        public void BindData(int? spamWordId)
        {
            this.SpamWordId = spamWordId;

            this.Title.LocalizedPage = "ADMIN_SPAMWORDS_EDIT";
            this.Save.TextLocalizedPage = "ADMIN_SPAMWORDS";

            if (this.SpamWordId.HasValue)
            {
                // Edit
                var spamWord = this.GetRepository<Spam_Words>().GetById(this.SpamWordId.Value);

                if (spamWord != null)
                {
                    this.spamword.Text = spamWord.SpamWord;
                }

                this.Title.LocalizedTag = "TITLE_EDIT";
                this.Save.TextLocalizedTag = "SAVE";
            }
            else
            {
                // Add
                this.spamword.Text = string.Empty;

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
            return !newExpression.Equals("*");
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsValidWordExpression(this.spamword.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_SPAMWORDS_EDIT", "MSG_REGEX_SPAM"),
                    MessageTypes.danger);
            }
            else
            {
                this.GetRepository<Spam_Words>().Save(
                     spamWordId: this.SpamWordId,
                     spamWord: this.spamword.Text);

                 this.Get<IDataCache>().Remove(Constants.Cache.SpamWords);

                 YafBuildLink.Redirect(ForumPages.admin_spamwords);
            }
        }

        #endregion
    }
}
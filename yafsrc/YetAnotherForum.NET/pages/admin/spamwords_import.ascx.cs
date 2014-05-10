/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
    using System.Data;
    using System.Linq;

    using YAF.Classes.Data;
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
    /// The Admin spam words import page.
    /// </summary>
    public partial class spamwords_import : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancel Import and Return Back to Previous Page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_spamwords);
        }

        /// <summary>
        /// Try to Import from selected File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Import_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // import selected file (if it's the proper format)...
            if (!this.importFile.PostedFile.ContentType.StartsWith("text"))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_SPAMWORDS_IMPORT", "MSG_IMPORTED_FAILEDX")
                        .FormatWith(
                            "Invalid upload format specified: {0}".FormatWith(this.importFile.PostedFile.ContentType)));

                return;
            }

            try
            {
                // import spam words...
                var dsSpamWords = new DataSet();
                dsSpamWords.ReadXml(this.importFile.PostedFile.InputStream);

                if (dsSpamWords.Tables["YafSpamWords"] != null
                    && dsSpamWords.Tables["YafSpamWords"].Columns["spamword"] != null)
                {
                    int importedCount = 0;

                    var spamWordsList = this.GetRepository<Spam_Words>().List();

                    // import any extensions that don't exist...
                    foreach (DataRow row in
                        dsSpamWords.Tables["YafSpamWords"].Rows.Cast<DataRow>()
                            .Where(
                                row => spamWordsList.Select("spamword = '{0}'".FormatWith(row["spamword"])).Length == 0)
                        )
                    {
                        // add this...
                        this.GetRepository<Spam_Words>().Save(spamWordID: null, spamWord: row["spamword"].ToString());
                        importedCount++;
                    }

                    this.PageContext.LoadMessage.AddSession(
                        importedCount > 0
                            ? this.GetText("ADMIN_SPAMWORDS_IMPORT", "MSG_IMPORTED").FormatWith(importedCount)
                            : this.GetText("ADMIN_SPAMWORDS_IMPORT", "MSG_NOTHING"),
                        importedCount > 0 ? MessageTypes.Success : MessageTypes.Warning);

                    YafBuildLink.Redirect(ForumPages.admin_spamwords);
                }
                else
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_SPAMWORDS_IMPORT", "MSG_IMPORTED_FAILED"));
                }
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_SPAMWORDS_IMPORT", "MSG_IMPORTED_FAILEDX").FormatWith(x.Message));
            }
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
                this.GetText("ADMIN_SPAMWORDS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_spamwords));
            this.PageLinks.AddLink(this.GetText("ADMIN_SPAMWORDS_IMPORT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_SPAMWORDS", "TITLE"),
                this.GetText("ADMIN_SPAMWORDS_IMPORT", "TITLE"));

            this.Import.Text = this.GetText("COMMON", "IMPORT");
            this.cancel.Text = this.GetText("COMMON", "CANCEL");
        }

        #endregion
    }
}
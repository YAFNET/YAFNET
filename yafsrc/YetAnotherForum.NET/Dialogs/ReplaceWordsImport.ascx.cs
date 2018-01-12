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
    using System.Data;
    using System.Linq;

    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The Admin ReplaceWords Import Dialog.
    /// </summary>
    public partial class ReplaceWordsImport : BaseUserControl
    {
        #region Methods

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
                    this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED_FAILEDX").FormatWith(
                        "Invalid upload format specified: " + this.importFile.PostedFile.ContentType));

                return;
            }

            try
            {
                // import replace words...
                var replaceWords = new DataSet();
                replaceWords.ReadXml(this.importFile.PostedFile.InputStream);

                if (replaceWords.Tables["YafReplaceWords"] != null
                    && replaceWords.Tables["YafReplaceWords"].Columns["badword"] != null
                    && replaceWords.Tables["YafReplaceWords"].Columns["goodword"] != null)
                {
                    var importedCount = 0;

                    var replaceWordsList = this.GetRepository<Replace_Words>().GetByBoardId();

                    // import any extensions that don't exist...
                    foreach (DataRow row in replaceWords.Tables["YafReplaceWords"].Rows)
                    {
                        if (!replaceWordsList.Any(w => w.BadWord == row["badword"].ToString() && w.GoodWord == row["goodword"].ToString()))
                        {
                            // add this...
                            this.GetRepository<Replace_Words>().Save(replaceWordId: null, badWord: row["badword"].ToString(), goodWord: row["goodword"].ToString());
                            importedCount++;
                        }
                    }

                    this.PageContext.AddLoadMessage(
                        importedCount > 0
                            ? this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED").FormatWith(importedCount)
                            : this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_NOTHING"),
                        importedCount > 0 ? MessageTypes.success : MessageTypes.warning);
                }
                else
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED_FAILED"));
                }
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED_FAILEDX").FormatWith(x.Message));
            }
        }

        #endregion
    }
}
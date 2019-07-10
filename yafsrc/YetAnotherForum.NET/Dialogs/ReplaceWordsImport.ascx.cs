/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using YAF.Core.BaseControls;
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
            if (!this.importFile.PostedFile.ContentType.StartsWith(value: "text"))
            {
                this.PageContext.AddLoadMessage(
                    message: string.Format(
                        format: this.GetText(page: "ADMIN_REPLACEWORDS_IMPORT", tag: "MSG_IMPORTED_FAILEDX"), arg0: "Invalid upload format specified: " + this.importFile.PostedFile.ContentType));

                return;
            }

            try
            {
                // import replace words...
                var replaceWords = new DataSet();
                replaceWords.ReadXml(stream: this.importFile.PostedFile.InputStream);

                if (replaceWords.Tables[name: "YafReplaceWords"] != null
                    && replaceWords.Tables[name: "YafReplaceWords"].Columns[name: "badword"] != null
                    && replaceWords.Tables[name: "YafReplaceWords"].Columns[name: "goodword"] != null)
                {
                    var importedCount = 0;

                    var replaceWordsList = this.GetRepository<Replace_Words>().GetByBoardId();

                    // import any extensions that don't exist...
                    foreach (DataRow row in replaceWords.Tables[name: "YafReplaceWords"].Rows)
                    {
                        if (!replaceWordsList.Any(predicate: w => w.BadWord == row[columnName: "badword"].ToString() && w.GoodWord == row[columnName: "goodword"].ToString()))
                        {
                            // add this...
                            this.GetRepository<Replace_Words>().Save(replaceWordId: null, badWord: row[columnName: "badword"].ToString(), goodWord: row[columnName: "goodword"].ToString());
                            importedCount++;
                        }
                    }

                    this.PageContext.AddLoadMessage(
                        message: importedCount > 0
                            ? string.Format(format: this.GetText(page: "ADMIN_REPLACEWORDS_IMPORT", tag: "MSG_IMPORTED"), arg0: importedCount)
                            : this.GetText(page: "ADMIN_REPLACEWORDS_IMPORT", tag: "MSG_NOTHING"),
                        messageType: importedCount > 0 ? MessageTypes.success : MessageTypes.warning);
                }
                else
                {
                    this.PageContext.AddLoadMessage(message: this.GetText(page: "ADMIN_REPLACEWORDS_IMPORT", tag: "MSG_IMPORTED_FAILED"));
                }
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage(
                    message: string.Format(format: this.GetText(page: "ADMIN_REPLACEWORDS_IMPORT", tag: "MSG_IMPORTED_FAILEDX"), arg0: x.Message));
            }
        }

        #endregion
    }
}
/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Dialogs;

using YAF.Core.Services.Import;

/// <summary>
/// The Banned Email Import Dialog.
/// </summary>
public partial class BannedEmailImport : BaseUserControl
{
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
            this.PageBoardContext.Notify(
                this.GetTextFormatted("IMPORT_FAILED", this.importFile.PostedFile.ContentType),
                MessageTypes.danger);

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("ImportDialog"));

            return;
        }

        try
        {
            var importedCount = DataImport.BannedEmailAddressesImport(
                this.PageBoardContext.PageBoardID,
                this.importFile.PostedFile.InputStream);

            this.PageBoardContext.Notify(
                importedCount > 0
                    ? string.Format(this.GetText("ADMIN_BANNEDEMAIL_IMPORT", "IMPORT_SUCESS"), importedCount)
                    : this.GetText("ADMIN_BANNEDEMAIL_IMPORT", "IMPORT_NOTHING"),
                MessageTypes.success);
        }
        catch (Exception x)
        {
            this.PageBoardContext.Notify(
                string.Format(this.GetText("ADMIN_BANNEDEMAIL_IMPORT", "IMPORT_FAILED"), x.Message), MessageTypes.danger);

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("ImportDialog"));
        }
    }
}
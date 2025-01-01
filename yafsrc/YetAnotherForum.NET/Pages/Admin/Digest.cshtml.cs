
/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Pages.Admin;

using System.Threading.Tasks;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Extensions;

/// <summary>
/// Manage Forum Digest Sending
/// </summary>
public class DigestModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public string TextSendEmail { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DigestModel"/> class.
    /// </summary>
    public DigestModel()
        : base("ADMIN_DIGEST", ForumPages.Admin_Digest)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_DIGEST", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Force Sending the Current Digest
    /// </summary>
    public IActionResult OnPostForceSend()
    {
        this.PageBoardContext.BoardSettings.ForceDigestSend = true;
        this.Get<BoardSettingsService>().SaveRegistry(this.PageBoardContext.BoardSettings);

        return this.PageBoardContext.Notify(this.GetText("ADMIN_DIGEST", "MSG_FORCE_SEND"), MessageTypes.success);
    }

    /// <summary>
    /// Send Test Digest
    /// </summary>
    public async Task<IActionResult> OnPostTestSendAsync()
    {
        if (!this.TextSendEmail.IsSet())
        {
            return this.PageBoardContext.Notify(this.GetText("ADMIN_DIGEST", "MSG_VALID_MAIL"), MessageTypes.danger);
        }

        try
        {
            // create and send a test digest to the email provided...
            var digestMail = this.Get<IDigestService>().CreateDigest(
                this.PageBoardContext.PageUser,
                new MailboxAddress(
                    this.PageBoardContext.BoardSettings.Name,
                    this.PageBoardContext.BoardSettings.ForumEmail),
                this.TextSendEmail.Trim(),
                string.Empty);

            await this.Get<IMailService>().SendAllAsync([digestMail]);

            return this.PageBoardContext.Notify(
                this.GetTextFormatted("MSG_SEND_SUC", "Direct"),
                MessageTypes.success);
        }
        catch (Exception ex)
        {
            return this.PageBoardContext.Notify(
                string.Format(this.GetText("ADMIN_DIGEST", "MSG_SEND_ERR"), ex),
                MessageTypes.danger);
        }
    }
}
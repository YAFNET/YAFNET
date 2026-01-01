/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
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

using System.Threading.Tasks;

namespace YAF.Pages.Profile;

using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// The edit user signature page
/// </summary>
public class EditSignatureModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "EditSignatureModel" /> class.
    /// </summary>
    public EditSignatureModel()
        : base("EDIT_SIGNATURE", ForumPages.Profile_EditSignature)
    {
    }

    /// <summary>
    /// Gets or sets the signature.
    /// </summary>
    [BindProperty]
    public string Signature { get; set; }

    /// <summary>
    ///   Gets or sets the number of characters which is allowed in user signature.
    /// </summary>
    [BindProperty]
    public int AllowedNumberOfCharacters { get; set; }

    /// <summary>
    ///   Gets or sets the string with allowed BBCodes info.
    /// </summary>
    [BindProperty]
    public string AllowedBbcodes { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("EDIT_SIGNATURE","TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        if (!this.PageBoardContext.BoardSettings.AllowSignatures &&
            !(this.PageBoardContext.IsAdmin || this.PageBoardContext.IsForumModerator))
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        await this.BindDataAsync(true);

        return this.Page();
    }

    /// <summary>
    /// Preview Signature
    /// </summary>
    public Task OnPostPreviewAsync()
    {
        return this.ValidateSignatureAsync();
    }

    /// <summary>
    /// Save the Signature.
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (this.Signature.Length > 0)
        {
            await this.ValidateSignatureAsync();

            if (this.Signature.Length <= this.AllowedNumberOfCharacters)
            {
                if (this.PageBoardContext.PageUser.NumPosts <
                    this.PageBoardContext.BoardSettings.IgnoreSpamWordCheckPostCount && this.Get<ISpamWordCheck>().CheckForSpamWord(this.Signature, out var result))
                {
                    // Check for spam
                    switch (this.PageBoardContext.BoardSettings.BotHandlingOnRegister)
                    {
                        // Log and Send Message to Admins
                        case 1:
                            this.Get<ILogger<EditSignatureModel>>().SpamBotDetected(
                                this.PageBoardContext.PageUserID,
                                $"""
                                 Internal Spam Word Check detected a SPAM BOT: (
                                                                                       user name : '{this.PageBoardContext.PageUser.Name}',
                                                                                       user id : '{this.PageBoardContext.PageUserID}')
                                                                                  after the user included a spam word in his/her signature: {result}
                                 """);
                            break;
                        case 2:
                        {
                            this.Get<ILogger<EditSignatureModel>>().SpamBotDetected(
                                this.PageBoardContext.PageUserID,
                                $"""
                                 Internal Spam Word Check detected a SPAM BOT: (
                                                                                        user name : '{this.PageBoardContext.PageUser.Name}',
                                                                                        user id : '{this.PageBoardContext.PageUserID}')
                                                                                  after the user included a spam word in his/her signature: {result}, user was deleted and the name, email and IP Address are banned.
                                 """);

                            await this.Get<IAspNetUsersHelper>().DeleteAndBanUserAsync(
                                this.PageBoardContext.PageUser,
                                this.PageBoardContext.MembershipUser,
                                this.PageBoardContext.PageUser.IP);

                            break;
                        }
                    }
                }

                await this.GetRepository<User>().SaveSignatureAsync(
                    this.PageBoardContext.PageUserID,
                    this.Signature);
            }
            else
            {
                return this.PageBoardContext.Notify(
                    this.GetTextFormatted("SIGNATURE_MAX", this.AllowedNumberOfCharacters),
                    MessageTypes.warning);
            }
        }
        else
        {
            await this.GetRepository<User>().SaveSignatureAsync(this.PageBoardContext.PageUserID, null);
        }

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.MyAccount);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="loadSignature">
    /// The load Signature.
    /// </param>
    private async Task BindDataAsync(bool loadSignature)
    {
        if (loadSignature)
        {
            this.Signature = this.PageBoardContext.PageUser.Signature;
        }

        var data = await this.GetRepository<User>().SignatureDataAsync(
            this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageBoardID);

        if (data is null)
        {
            return;
        }

        this.AllowedBbcodes = (string)data.UsrSigBBCodes.Trim().Trim(',').Trim();

        this.AllowedNumberOfCharacters = (int)data.UsrSigChars;
    }

    /// <summary>
    /// The validate signature.
    /// </summary>
    private async Task ValidateSignatureAsync()
    {
        await this.BindDataAsync(false);

        var body = HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.Signature ?? string.Empty));

        // find forbidden BBCodes in signature
        var detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this.AllowedBbcodes, ',');

        if (this.AllowedBbcodes.IndexOf("ALL", StringComparison.Ordinal) < 0)
        {
            if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
            {
                this.PageBoardContext.Notify(
                    this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode),
                    MessageTypes.warning);
                return;
            }

            if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
            {
                this.PageBoardContext.Notify(this.GetText("BBCODE_FORBIDDEN"), MessageTypes.warning);
                return;
            }
        }

        if (this.Signature.Length <= this.AllowedNumberOfCharacters)
        {
            this.Signature = this.Get<IBadWordReplace>().Replace(body);
        }
        else
        {
            this.PageBoardContext.Notify(
                this.GetTextFormatted("SIGNATURE_MAX", this.AllowedNumberOfCharacters),
                MessageTypes.warning);
        }
    }
}